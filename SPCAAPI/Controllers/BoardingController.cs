using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using Firebase.Storage;
using SPCAAPI.Models;

namespace SPCAAPI.Controllers
{
    [Route("api/Boarding")]
    [ApiController]
    public class BoardingController : Controller
    {
        public static FirestoreDb db = AnimalController.establishCon();

        // Method to add a new boarding request to the Firestore database
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] BoardingRequest boardingRequest)
        {
            try
            {
                if (boardingRequest == null)
                {
                    return BadRequest(new { message = "Invalid boarding request data" });
                }

               
                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    if (file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var stream = file.OpenReadStream();
                        var firebaseStorage = new FirebaseStorage("wilspca.appspot.com");

                        var uploadTask = firebaseStorage
                            
                            .Child(fileName)
                            .PutAsync(stream);

                       
                    }
                }

                CollectionReference coll = db.Collection("BoardingRequests");
                DocumentReference docRef = coll.Document();

                Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "petName", boardingRequest.PetName },
            { "breed", boardingRequest.Breed },
            { "startDate", boardingRequest.StartDate },
            { "endDate", boardingRequest.EndDate },
            { "ownerName", boardingRequest.OwnerName },
            { "ownerEmail", boardingRequest.OwnerEmail },
            
        };

                await docRef.SetAsync(data);

                return Ok(new { message = "Boarding request submitted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error processing the request: {ex.Message}" });
            }
        }


        // Method to get all boarding requests from Firestore
        [HttpGet("GetBoardingRequests")]
        public async Task<IActionResult> GetBoardingRequests()
        {
            Query qRef = db.Collection("BoardingRequests");
            QuerySnapshot snapshot = await qRef.GetSnapshotAsync();

            if (snapshot == null || snapshot.Count == 0)
            {
                return NotFound(new { message = "No boarding requests found." });
            }

            List<Dictionary<string, object>> boardingRequests = new List<Dictionary<string, object>>();

            foreach (DocumentSnapshot docsnap in snapshot)
            {
                Dictionary<string, object> entry = docsnap.ConvertTo<Dictionary<string, object>>();
                entry.Add("requestId", docsnap.Reference.Id.ToString());

                if (docsnap.Exists)
                {
                    boardingRequests.Add(entry);
                }
            }

            return Ok(boardingRequests);
        }

        // Method to delete a boarding request from Firestore
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                // Get reference to the document in the Firestore
                DocumentReference docRef = db.Collection("BoardingRequests").Document(id);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                // Check if the boarding request exists
                if (!snapshot.Exists)
                {
                    return NotFound(new { message = "Boarding request not found" });
                }

              

                    var firebaseStorage = new FirebaseStorage("wilspca.appspot.com");
                    
                

                // Delete the boarding request from Firestore
                await docRef.DeleteAsync();
                return Ok(new { message = "Boarding request deleted successfully" });
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return StatusCode(500, new { message = $"Error deleting boarding request: {ex.Message}" });
            }
        }

    }
}
