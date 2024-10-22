using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using SPCAAPI.Models;

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
                Query query = db.Collection("Users").WhereEqualTo("email", user.Email);
                QuerySnapshot snapshot = await query.GetSnapshotAsync();

                if (snapshot.Documents.Count > 0)
                {
                    return BadRequest(new { message = "Error, email in use." });
                }

                // Add the user to the Firestore collection
                CollectionReference coll = db.Collection("Users");
                DocumentReference docRef = coll.Document(user.Email);

                Dictionary<string, object> data = new Dictionary<string, object>
                {
                    { "email", user.Email },
                    { "password", user.Password },
                    { "usertype", "User" },
                };

                await docRef.SetAsync(data);

                return Ok(new { message = "User created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error processing the request: {ex.Message}" });
            }
        }

        // Method to check if email is in use
        [HttpGet("FindEmail/{email}")]
        public async Task<IActionResult> FindEmail(string email)
        {
            Query query = db.Collection("Users").WhereEqualTo("email", email);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            if (snapshot.Documents.Count > 0)
            {
                return Ok(new { message = "Error, email in use." });
            }
            return Ok(new { message = "Email fine" });
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
                if (userInfo["password"].ToString() == password)
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
    }
}
