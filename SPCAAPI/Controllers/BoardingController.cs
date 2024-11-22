using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using Firebase.Storage;
using SPCAAPI.Models;
using SPCAAPI.Data;

namespace SPCAAPI.Controllers
{
    [Route("api/Boarding")]
    [ApiController]
    public class BoardingController : Controller
    {
        public static FirestoreDb db = AnimalController.establishCon();
        WilDbContext context = new WilDbContext();

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] BoardingRequest boardingRequest)
        {
            try
            {
                context.BoardingRequests.Add(boardingRequest);
                context.SaveChanges();

                return Ok(new { message = "Boarding request submitted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error processing the request: {ex.Message}" });
            }
        }

        [HttpGet("GetBoardingRequests")]
        public async Task<IActionResult> GetBoardingRequests()
        {
            var requests = context.BoardingRequests.ToList();

            if (requests == null || requests.Count == 0)
            {
                return NotFound(new { message = "No boarding requests found." });
            }

            return Ok(requests);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var request = context.BoardingRequests.Where(x => x.BoardingId == id).FirstOrDefault();

                if (request == null)
                {
                    return NotFound(new { message = "Boarding request not found" });
                }

                context.BoardingRequests.Remove(request);
                context.SaveChanges();

                return Ok(new { message = "Boarding request deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error deleting boarding request: {ex.Message}" });
            }
        }
        [HttpPut("UpdateBoarding/{id}")]
        public async Task<IActionResult> UpdateBoarding(int id, [FromForm] BoardingRequest updatedRequest)
        {
            try
            {
                var request = context.BoardingRequests.Where(x => x.BoardingId == id).FirstOrDefault();

                if (request == null)
                {
                    return NotFound(new { message = "Event not found" });
                }

                request.StartDate = updatedRequest.StartDate;
                request.EndDate = updatedRequest.EndDate;
                request.OwnerName = updatedRequest.OwnerName;
                request.OwnerEmail = updatedRequest.OwnerEmail;
                request.PetName = updatedRequest.PetName;
                request.Breed = updatedRequest.Breed;
                context.BoardingRequests.Update(request);
                context.SaveChanges();

                return Ok(new { message = "Event updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error updating event: {ex.Message}" });
            }
        }
    }
}
