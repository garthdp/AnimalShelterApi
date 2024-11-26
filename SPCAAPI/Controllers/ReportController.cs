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
        private readonly WilDbContext _context;
        public ReportController(WilDbContext context)
        {
            _context = context;
        }

        // Method to create a new report
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] Report report)
        {
            try
            {
                if (report == null)
                {
                    return BadRequest(new { message = "Incorrect report format." });
                }

                _context.Reports.Add(report);
                _context.SaveChanges();

                return Ok(new { message = "Report submitted successfully"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error processing the request: {ex.Message}" });
            }
        }

        // Method to get all reports
        [HttpGet("GetReports")]
        public async Task<IActionResult> GetReports()
        {
            var reports = _context.Reports.ToList();

            if (reports == null || reports.Count == 0)
            {
                return NotFound(new { message = "No reports found." });
            }

            return Ok(reports);
        }

        // method to get user reports
        [HttpGet("UserReports/{email}")]
        public async Task<IActionResult> UserReports(string email)
        {
            var reports = _context.Reports.Where(x => x.ContactInfo == email).ToList();

            if (reports == null || reports.Count == 0)
            {
                return NotFound(new { message = "No reports found." });
            }

            return Ok(reports);
        }
        // method to delete a report
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var report = _context.Reports.Where(x => x.ReportId == id).FirstOrDefault();

            if (report != null)
            {
                _context.Reports.Remove(report);
                _context.SaveChanges();
                return Ok(new { message = "Report deleted" });
            }
            else
            {
                return NotFound(new { message = "Report not found" });
            }
        }
    }
}
