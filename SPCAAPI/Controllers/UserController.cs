using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using SPCAAPI.Models;
using BCrypt.Net;
using Firebase.Storage;

namespace SPCAAPI.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class UserController : Controller
    {
        public static FirestoreDb db = AnimalController.establishCon(); // Adjust this for your Firestore initialization

        // Method to register a user 
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] User user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest(new { message = "Invalid user data" });
                }
                Query query = db.Collection("Users").WhereEqualTo("Email", user.UserEmail);
                QuerySnapshot snapshot = await query.GetSnapshotAsync();

                if (snapshot.Documents.Count > 0)
                {
                    return BadRequest(new { message = "Error, email in use." });
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

                // Add the user to the Firestore collection
                CollectionReference coll = db.Collection("Users");
                DocumentReference docRef = coll.Document(user.UserEmail);

                Dictionary<string, object> data = new Dictionary<string, object>
                {
                    { "Email", user.UserEmail },
                    { "Password", hashedPassword },
                    { "UserType", "User" },
                    { "FirstName", user.FirstName },
                    { "LastName", user.LastName },
                    { "PhoneNumber", "" },
                    { "City", "" },
                    { "Address", "" },
                    { "imageUrl", "https://firebasestorage.googleapis.com/v0/b/wilspca.appspot.com/o/animal_images%2FSPCALOGO.jpg?alt=media&token=8b42660f-821c-4939-aa9e-6aca76cc1bed" },
                };

                await docRef.SetAsync(data);

                return Ok(new { message = "User created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error processing the request: {ex.Message}" });
            }
        }

        // Method to login
        [HttpGet("Login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            DocumentReference docRef = db.Collection("Users").Document(email);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> userInfo = snapshot.ConvertTo<Dictionary<string, object>>();
                string storedHashedPassword = userInfo["Password"].ToString();
                if (BCrypt.Net.BCrypt.Verify(password, storedHashedPassword))
                {
                    return Ok(userInfo);
                }
                else
                {
                    return NotFound(new { message = "Email or password incorrect"});
                }
            }
            else
            {
                return NotFound(new { message = "Email or password incorrect"});
            }
        }

        // Method to login
        [HttpGet("GetInfo/{email}")]
        public async Task<IActionResult> GetInfo(string email)
        {
            DocumentReference docRef = db.Collection("Users").Document(email);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> userInfo = snapshot.ConvertTo<Dictionary<string, object>>();
                return Ok(userInfo);
            }
            else
            {
                return NotFound(new { message = "Failed to load." });
            }
        }


        // Method to delete a user
        [HttpDelete("DeleteUser/{email}")]
        public async Task<IActionResult> DeleteEvent(string email)
        {
            try
            {
                DocumentReference docRef = db.Collection("Users").Document(email);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    return NotFound(new { message = "User not found" });
                }

                await docRef.DeleteAsync();
                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error deleting user: {ex.Message}" });
            }
        }
        [HttpPatch("Update/{email}")]
        public async Task<IActionResult> Patch(string email, [FromForm] User user)
        {
            DocumentReference docRef = db.Collection("Users").Document(email);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                return NotFound(new { message = "User not found" });
            }

            Dictionary<string, object> updates = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(user.FirstName))
            {
                updates["FirstName"] = user.FirstName;
            }
            if (!string.IsNullOrEmpty(user.LastName))
            {
                updates["LastName"] = user.LastName;
            }
            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                updates["PhoneNumber"] = user.PhoneNumber;
            }
            if (!string.IsNullOrEmpty(user.City))
            {
                updates["City"] = user.City;
            }
            if (!string.IsNullOrEmpty(user.Address))
            {
                updates["Address"] = user.Address;
            }
            if (updates.Count > 0)
            {
                await docRef.UpdateAsync(updates);
                return Ok(new { message = "Animal updated", updates });
            }

            return BadRequest(new { message = "No updates provided" });
        }
    }
}
