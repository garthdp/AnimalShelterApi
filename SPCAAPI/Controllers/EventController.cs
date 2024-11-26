using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using SPCAAPI.Models; // Assuming you have a model for Event
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using SPCAAPI.Data;

namespace SPCAAPI.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventController : Controller
    {
        private readonly WilDbContext _context;
        public EventController(WilDbContext context)
        {
            _context = context;
        }

        // Method to add a new event
        [HttpPost("AddEvent")]
        public async Task<IActionResult> AddEvent([FromForm] Events newEvent)
        {
            try
            {
                if (newEvent == null)
                {
                    return BadRequest(new { message = "Invalid event data" });
                }

                Event addEvent = new Event();
                addEvent.EventName = newEvent.eventName;
                addEvent.EventDate = newEvent.eventDate;
                addEvent.EventDescription = newEvent.eventDescription;

                _context.Events.Add(addEvent);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Event added successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error processing the request: {ex.Message}" });
            }
        }

        // Method to get all events from database
        [HttpGet("GetEvents")]
        public async Task<IActionResult> GetEvents()
        {

            var FindEvents = _context.Events.ToList();

            if (FindEvents == null || FindEvents.Count == 0)
            {
                return NotFound(new { message = "No events found." });
            }

            return Ok(FindEvents);
        }

        // Method to get a specific event
        [HttpGet("GetEvent/{id}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var ev = _context.Events.Where(x => x.EventId == id).FirstOrDefault();

            if (ev != null)
            {
                return Ok(ev);
            }
            else
            {
                return NotFound(new { message = "Event not found" });
            }
        }

        // Method to update an event
        [HttpPut("UpdateEvent/{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromForm] Events updatedEvent)
        {
            try
            {
                var ev = _context.Events.Where(x => x.EventId == id).FirstOrDefault();

                if (ev == null)
                {
                    return NotFound(new { message = "Event not found" });
                }

                ev.EventDate = updatedEvent.eventDate;
                ev.EventDescription = updatedEvent.eventDescription;
                ev.EventName = updatedEvent.eventName;
                _context.Events.Update(ev);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Event updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error updating event: {ex.Message}" });
            }
        }

        // Method to delete an event
        [HttpDelete("DeleteEvent/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                var ev = _context.Events.Where(x => x.EventId == id).FirstOrDefault();

                if (ev == null)
                {
                    return NotFound(new { message = "Event not found" });
                }

                _context.Events.Remove(ev);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Event deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error deleting event: {ex.Message}" });
            }
        }
    }
}
