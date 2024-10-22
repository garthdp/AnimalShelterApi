using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using SPCAAPI.Models;

namespace SPCAAPI.Controllers
{
    [Route("api/Volunteer")]
    [ApiController]
    public class VolunteerController : Controller
    {
        public static FirestoreDb db = AnimalController.establishCon();

        [HttpPost("AddVolunteer")]
        public async Task<IActionResult> AddVolunteer([FromForm] Volunteer volunteer)
        {
            try
            {
                if (volunteer == null)
                {
                    return BadRequest(new { message = "Invalid volunteer data" });
                }

                // Add the event to the Firestore collection
                CollectionReference coll = db.Collection("Volunteers");
                DocumentReference docRef = coll.Document();

                Dictionary<string, object> data = new Dictionary<string, object>
                {
                    {"Name", volunteer.Name },
                    {"Surname", volunteer.Surname },
                    {"VolunteerDate", volunteer.VolunteerDate },
                    {"PhoneNumber", volunteer.PhoneNumber },
                    {"Email", volunteer.Email }
                };

                await docRef.SetAsync(data);

                return Ok(new { message = "Volunteer added successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error processing the request: {ex.Message}" });
            }
        }
        [HttpGet("GetVolunteers")]
        public async Task<IActionResult> GetVolunteers()
        {
            Query qRef = db.Collection("Volunteers");
            QuerySnapshot snapshot = await qRef.GetSnapshotAsync();

            if (snapshot == null || snapshot.Count == 0)
            {
                return NotFound(new { message = "No volunteers found." });
            }

            List<Dictionary<string, object>> volunteers = new List<Dictionary<string, object>>();

            foreach (DocumentSnapshot docsnap in snapshot)
            {
                Dictionary<string, object> entry = docsnap.ConvertTo<Dictionary<string, object>>();
                entry.Add("volunteerId", docsnap.Reference.Id.ToString());

                if (docsnap.Exists)
                {
                    volunteers.Add(entry);
                }
            }

            return Ok(volunteers);
        }
        // Method to update an volunteer
        [HttpPut("UpdateVolunteer/{id}")]
        public async Task<IActionResult> UpdateVolunteer(string id, [FromForm] Volunteer volunteer)
        {
            try
            {
                DocumentReference docRef = db.Collection("Volunteers").Document(id);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    return NotFound(new { message = "Volunteer not found" });
                }

                Dictionary<string, object> updatedData = new Dictionary<string, object>
                {
                    {"Name", volunteer.Name },
                    {"Surname", volunteer.Surname },
                    {"VolunteerDate", volunteer.VolunteerDate },
                    {"PhoneNumber", volunteer.PhoneNumber },
                    {"Email", volunteer.Email }
                };

                await docRef.UpdateAsync(updatedData);
                return Ok(new { message = "Volunteer updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error updating event: {ex.Message}" });
            }
        }
        // Method to delete an Volunteer
        [HttpDelete("DeleteVolunteer/{id}")]
        public async Task<IActionResult> DeleteEvent(string id)
        {
            try
            {
                DocumentReference docRef = db.Collection("Volunteers").Document(id);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    return NotFound(new { message = "Volunteer not found" });
                }

                await docRef.DeleteAsync();
                return Ok(new { message = "Volunteer deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error deleting event: {ex.Message}" });
            }
        }
    }
}
