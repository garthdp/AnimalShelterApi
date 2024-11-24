using Microsoft.AspNetCore.Mvc;
using SPCAAPI.Models;
using BCrypt.Net;
using SPCAAPI.Data;

namespace SPCAAPI.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly WilDbContext _context;
        public UserController(WilDbContext context)
        {
            _context = context;
        }

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
                var checkEmail = _context.Users.Where(x => x.UserEmail == user.UserEmail).FirstOrDefault();

                if (checkEmail != null)
                {
                    return BadRequest(new { message = "Error, email in use." });
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

                user.Password = hashedPassword;
                user.Address = "";
                user.ProfilePicture = "";
                user.PhoneNumber = "";
                user.UserType = "User";
                user.City = "";

                _context.Users.Add(user);
                _context.SaveChanges();

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
            var user = _context.Users.Where(x => x.UserEmail == email).FirstOrDefault();

            if (user != null)
            {

                string storedHashedPassword = user.Password;
                if (BCrypt.Net.BCrypt.Verify(password, storedHashedPassword))
                {
                    User userInfo = new User();
                    userInfo.UserEmail = email;
                    userInfo.UserType = user.UserType;
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
            var user = _context.Users.Where(x => x.UserEmail == email).FirstOrDefault();

            if (user != null)
            {
                return Ok(user);
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
                var user = _context.Users.Where(x => x.UserEmail == email).FirstOrDefault();

                if (user != null)
                {
                    return NotFound(new { message = "User not found" });
                }

                _context.Users.Remove(user);
                _context.SaveChanges();
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
            var foundUser = _context.Users.Where(x => x.UserEmail == email).FirstOrDefault();

            if (foundUser == null)
            {
                return NotFound(new { message = "User not found" });
            }

            if (!string.IsNullOrEmpty(user.FirstName))
            {
                foundUser.FirstName = user.FirstName;
            }
            if (!string.IsNullOrEmpty(user.LastName))
            {
                foundUser.LastName = user.LastName;
            }
            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                foundUser.PhoneNumber = user.PhoneNumber;
            }
            if (!string.IsNullOrEmpty(user.City))
            {
                foundUser.City = user.City;
            }
            if (!string.IsNullOrEmpty(user.Address))
            {
                foundUser.Address = user.Address;
            }

            _context.Users.Update(foundUser);
            _context.SaveChanges();
            return Ok(new { message = "User updated", foundUser });
        }
    }
}
