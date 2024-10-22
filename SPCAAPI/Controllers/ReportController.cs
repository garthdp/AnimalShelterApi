using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using Firebase.Storage;
using SPCAAPI.Models;

namespace SPCAAPI.Controllers
{
    [Route("api/Report")]
    [ApiController]
    public class ReportController : Controller
    {
        public static FirestoreDb db = AnimalController.establishCon();

        // Method to add a new report to the Firestore database
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] Report report)
        {
            try
            {
                // Check if the report is valid
                if (report == null)
                {
                    return BadRequest(new { message = "Invalid report data" });
                }

                // If the report has an image, upload it to Firebase Storage
                string imageUrl = null;
                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    if (file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var stream = file.OpenReadStream();
                        var firebaseStorage = new FirebaseStorage("wilspca.appspot.com");

                        var uploadTask = firebaseStorage
                            .Child("report_images") // Store images in a folder for reports
                            .Child(fileName)
                            .PutAsync(stream);

                        imageUrl = await uploadTask;
                    }
                }

                // Add the report to the Firestore collection
                CollectionReference coll = db.Collection("Reports");
                DocumentReference docRef = coll.Document();

                Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "location", report.Location },
            { "description", report.Description },
            { "contactInfo", report.ContactInfo },
            { "status", report.Status },
            { "imageUrl", imageUrl }
        };

                await docRef.SetAsync(data);

                return Ok(new { message = "Report submitted successfully", imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error processing the request: {ex.Message}" });
            }
        }

        // Method to get all reports from Firestore
        [HttpGet("GetReports")]
        public async Task<IActionResult> GetReports()
        {
            Query qRef = db.Collection("Reports");
            QuerySnapshot snapshot = await qRef.GetSnapshotAsync();

            if (snapshot == null || snapshot.Count == 0)
            {
                return NotFound(new { message = "No reports found." });
            }

            List<Dictionary<string, object>> reports = new List<Dictionary<string, object>>();

            foreach (DocumentSnapshot docsnap in snapshot)
            {
                Dictionary<string, object> entry = docsnap.ConvertTo<Dictionary<string, object>>();
                entry.Add("reportId", docsnap.Reference.Id.ToString());

                if (docsnap.Exists)
                {
                    reports.Add(entry);
                }
            }

            return Ok(reports);
        }

        // Method to delete a report from Firestore
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            DocumentReference docRef = db.Collection("Reports").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                string imageUrl = snapshot.GetValue<string>("imageUrl");

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    // Format the string to get to the Firebase folder
                    var imagePath = imageUrl.Substring(imageUrl.IndexOf("o/") + 2);
                    imagePath = imagePath.Substring(0, imagePath.IndexOf("?alt="));
                    imagePath = imagePath.Replace("%2F", "/");

                    var firebaseStorage = new FirebaseStorage("wilspca.appspot.com");
                    try
                    {
                        await firebaseStorage
                            .Child(imagePath)
                            .DeleteAsync();
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new { message = $"Error deleting image: {ex.Message}" });
                    }
                }

                await docRef.DeleteAsync();
                return Ok(new { message = "Report and associated image deleted" });
            }
            else
            {
                return NotFound(new { message = "Report not found" });
            }
        }
    }
}
