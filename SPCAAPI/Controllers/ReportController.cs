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
                // Add the report to the Firestore collection
                CollectionReference coll = db.Collection("Reports");
                DocumentReference docRef = coll.Document();

                Dictionary<string, object> data = new Dictionary<string, object>
                {
                    { "location", report.Location },
                    { "description", report.Description },
                    { "contactInfo", report.ContactInfo },
                    { "status", report.Status },
                };

                await docRef.SetAsync(data);

                return Ok(new { message = "Report submitted successfully"});
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
