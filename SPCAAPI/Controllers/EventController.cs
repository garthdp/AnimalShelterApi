using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using SPCAAPI.Models; // Assuming you have a model for Event
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

namespace SPCAAPI.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventController : Controller
    {
        public static FirestoreDb db = AnimalController.establishCon(); // Adjust this for your Firestore initialization

        // Method to add a new event to Firestore
        [HttpPost("AddEvent")]
        public async Task<IActionResult> AddEvent([FromForm] Events newEvent)
        {
            try
            {
                if (newEvent == null)
                {
                    return BadRequest(new { message = "Invalid event data" });
                }

                // Add the event to the Firestore collection
                CollectionReference coll = db.Collection("Events");
                DocumentReference docRef = coll.Document();

                Dictionary<string, object> data = new Dictionary<string, object>
                {
                    { "eventName", newEvent.Title },
                    { "eventDescription", newEvent.Description },
                    { "eventDate", newEvent.Date },
                };

                await docRef.SetAsync(data);

                return Ok(new { message = "Event added successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error processing the request: {ex.Message}" });
            }
        }

        // Method to get all events from Firestore
        [HttpGet("GetEvents")]
        public async Task<IActionResult> GetEvents()
        {
            Query qRef = db.Collection("Events");
            QuerySnapshot snapshot = await qRef.GetSnapshotAsync();

            if (snapshot == null || snapshot.Count == 0)
            {
                return NotFound(new { message = "No events found." });
            }

            List<Dictionary<string, object>> events = new List<Dictionary<string, object>>();

            foreach (DocumentSnapshot docsnap in snapshot)
            {
                Dictionary<string, object> entry = docsnap.ConvertTo<Dictionary<string, object>>();
                entry.Add("eventId", docsnap.Reference.Id.ToString());

                if (docsnap.Exists)
                {
                    events.Add(entry);
                }
            }

            return Ok(events);
        }

        // Method to get a specific event by ID from Firestore
        [HttpGet("GetEvent/{id}")]
        public async Task<IActionResult> GetEvent(string id)
        {
            DocumentReference docRef = db.Collection("Events").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> eventEntry = snapshot.ConvertTo<Dictionary<string, object>>();
                eventEntry.Add("eventId", snapshot.Reference.Id.ToString());
                return Ok(eventEntry);
            }
            else
            {
                return NotFound(new { message = "Event not found" });
            }
        }

        // Method to update an event
        [HttpPut("UpdateEvent/{id}")]
        public async Task<IActionResult> UpdateEvent(string id, [FromForm] Events updatedEvent)
        {
            try
            {
                DocumentReference docRef = db.Collection("Events").Document(id);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    return NotFound(new { message = "Event not found" });
                }

                Dictionary<string, object> updatedData = new Dictionary<string, object>
                {
                    { "eventName", updatedEvent.Title },
                    { "eventDescription", updatedEvent.Description },
                    { "eventDate", updatedEvent.Date },
                };

                await docRef.UpdateAsync(updatedData);
                return Ok(new { message = "Event updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error updating event: {ex.Message}" });
            }
        }

        // Method to delete an event
        [HttpDelete("DeleteEvent/{id}")]
        public async Task<IActionResult> DeleteEvent(string id)
        {
            try
            {
                DocumentReference docRef = db.Collection("Events").Document(id);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    return NotFound(new { message = "Event not found" });
                }

                await docRef.DeleteAsync();
                return Ok(new { message = "Event deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error deleting event: {ex.Message}" });
            }
        }
    }
}
