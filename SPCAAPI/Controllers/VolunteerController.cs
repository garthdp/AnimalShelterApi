using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using SPCAAPI.Data;
using SPCAAPI.Models;

namespace SPCAAPI.Controllers
{
    [Route("api/Volunteer")]
    [ApiController]
    public class VolunteerController : Controller
    {
        public static FirestoreDb db = AnimalController.establishCon(); // Adjust this for your Firestore initialization
        WilDbContext context = new WilDbContext();

        [HttpPost("AddVolunteer")]
        public async Task<IActionResult> AddVolunteer([FromForm] Volunteer volunteer)
        {
            try
            {
                if (volunteer == null)
                {
                    return BadRequest(new { message = "Invalid volunteer data" });
                }

                context.Volunteers.Add(volunteer);
                context.SaveChanges();

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
            var volunteers = context.Volunteers.ToList();

            if (volunteers == null || volunteers.Count == 0)
            {
                return NotFound(new { message = "No volunteers found." });
            }

            return Ok(volunteers);
        }
        // Method to update an volunteer
        [HttpPut("UpdateVolunteer/{id}")]
        public async Task<IActionResult> UpdateVolunteer(int id, [FromForm] Volunteer volunteer)
        {
            try
            {
                var vol = context.Volunteers.Where(x => x.VolunteerId == id).FirstOrDefault();

                if (vol == null)
                {
                    return NotFound(new { message = "Volunteer not found" });
                }

                vol.Name = volunteer.Name;
                vol.Surname = volunteer.Surname;
                vol.VounteerDate = volunteer.VounteerDate;
                vol.PhoneNumber = volunteer.PhoneNumber;
                vol.Email = volunteer.Email;

                context.Update(vol);
                context.SaveChanges();

                return Ok(new { message = "Volunteer updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error updating event: {ex.Message}" });
            }
        }
        // Method to delete an Volunteer
        [HttpDelete("DeleteVolunteer/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                var vol = context.Volunteers.Where(x => x.VolunteerId == id).FirstOrDefault();

                if (vol == null)
                {
                    return NotFound(new { message = "Volunteer not found" });
                }

                context.Volunteers.Remove(vol);
                context.SaveChanges();

                return Ok(new { message = "Volunteer deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error deleting event: {ex.Message}" });
            }
        }
    }
}
