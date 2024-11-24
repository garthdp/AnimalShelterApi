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
        private readonly WilDbContext _context;
        public BoardingController(WilDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] BoardingRequest boardingRequest)
        {
            try
            {
                _context.BoardingRequests.Add(boardingRequest);
                await _context.SaveChangesAsync();

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
            var requests = _context.BoardingRequests.ToList();

            if (requests == null)
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
                var request = _context.BoardingRequests.Where(x => x.BoardingId == id).FirstOrDefault();

                if (request == null)
                {
                    return NotFound(new { message = "Boarding request not found" });
                }

                _context.BoardingRequests.Remove(request);
                await _context.SaveChangesAsync();

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
                var request = _context.BoardingRequests.Where(x => x.BoardingId == id).FirstOrDefault();

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
                _context.BoardingRequests.Update(request);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Event updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error updating event: {ex.Message}" });
            }
        }
    }
}
