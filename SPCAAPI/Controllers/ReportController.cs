using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using Firebase.Storage;
using SPCAAPI.Models;
using SPCAAPI.Data;

namespace SPCAAPI.Controllers
{
    [Route("api/Report")]
    [ApiController]
    public class ReportController : Controller
    {
        public static FirestoreDb db = AnimalController.establishCon();
        WilDbContext context = new WilDbContext();

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] Report report)
        {
            try
            {
                if (report == null)
                {
                    return BadRequest(new { message = "Incorrect report format." });
                }

                context.Reports.Add(report);
                context.SaveChanges();

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
            var reports = context.Reports.ToList();

            if (reports == null || reports.Count == 0)
            {
                return NotFound(new { message = "No reports found." });
            }

            return Ok(reports);
        }

        [HttpGet("UserReports/{email}")]
        public async Task<IActionResult> UserReports(string email)
        {
            var reports = context.Reports.Where(x => x.ContactInfo == email).ToList();

            if (reports == null || reports.Count == 0)
            {
                return NotFound(new { message = "No reports found." });
            }

            return Ok(reports);
        }

        // Method to delete a report from Firestore
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var report = context.Reports.Where(x => x.ReportId == id).FirstOrDefault();

            if (report != null)
            {
                context.Reports.Remove(report);
                context.SaveChanges();
                return Ok(new { message = "Report deleted" });
            }
            else
            {
                return NotFound(new { message = "Report not found" });
            }
        }
    }
}
