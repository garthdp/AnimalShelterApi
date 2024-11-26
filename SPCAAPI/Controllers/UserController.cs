using Microsoft.AspNetCore.Mvc;
using SPCAAPI.Models;
using BCrypt.Net;
using SPCAAPI.Data;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using Aes = System.Security.Cryptography.Aes;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SPCAAPI.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly WilDbContext _context;
        private readonly IConfiguration _configuration;
        public UserController(WilDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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

                // hashes password
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

                user.Password = hashedPassword;
                user.Address = "";
                user.ProfilePicture = "https://spcablob.blob.core.windows.net/users/logo.png";
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
                // checks if passwords match
                if (BCrypt.Net.BCrypt.Verify(password, storedHashedPassword))
                {
                    User userInfo = new User();
                    userInfo.UserEmail = user.UserEmail;
                    userInfo.UserType = user.UserType;

                    // generates jwt token
                    var token = GenerateJwtToken(userInfo);

                    // Can I create a cookie in a globally available static class?
                    // link = https://stackoverflow.com/questions/61584922/can-i-create-a-cookie-in-a-globally-available-static-class
                    // author = Mertuarez
                    // author link = https://stackoverflow.com/users/1071165/mertuarez
                    // learned how to make a cookie with certain options

                    return Ok(new {token});
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
        // Method to check admin access
        [HttpGet("CheckAdminAccess/{email}")]
        public async Task<IActionResult> CheckAdminAccess(string email)
        {
            var user = _context.Users.Where(x => x.UserEmail == email).FirstOrDefault();

            if (user != null)
            {
                if (user.UserType == "Admin")
                {
                    return Ok(new { message = "Success" });
                }
                else
                {
                    return BadRequest(new { message = "Do not have access." });
                }
            }
            else
            {
                return BadRequest(new { message = "Not logged in." });
            }
        }

        // Method to check user access
        [HttpGet("CheckUserAccess/{email}")]
        public async Task<IActionResult> CheckUserAccess(string email)
        {
            var user = _context.Users.Where(x => x.UserEmail == email).FirstOrDefault();

            if (user != null)
            {
                if (user.UserType == "User")
                {
                    return Ok(new { message = "Success" });
                }
                else
                {
                    return BadRequest(new { message = "Do not have access." });
                }
            }
            else
            {
                return BadRequest(new { message = "Not logged in." });
            }
        }
        // Method to user info
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
        public async Task<IActionResult> DeleteUser(string email)
        {
            try
            {
                var user = _context.Users.Where(x => x.UserEmail == email).FirstOrDefault();

                if (user != null)
                {
                    return NotFound(new { message = "User not found" });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error deleting user: {ex.Message}" });
            }
        }
        // method to update user information
        [HttpPatch("Update/{email}")]
        public async Task<IActionResult> Patch(string email, [FromForm] RecieveUser user)
        {
            var foundUser = _context.Users.Where(x => x.UserEmail == email).FirstOrDefault();

            if (foundUser == null)
            {
                return NotFound(new { message = "User not found" });
            }

            if (user.file != null && user.file.Length > 0)
            {
                if (!string.IsNullOrEmpty(user.ProfilePicture))
                {
                    try
                    {
                        // deletes previous profile picture
                        await DeleteBlobAsync(user.ProfilePicture, _configuration);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new { message = $"Error deleting old image: {ex.Message}" });
                    }
                }
                try
                {
                    // uploads new image
                    var newImageUrl = await UploadFileToBlobAsync(user.file, "users", _configuration);
                    foundUser.ProfilePicture = newImageUrl;
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = $"Error uploading new image: {ex.Message}" });
                }
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
            await _context.SaveChangesAsync();
            return Ok(new { message = "User updated", foundUser });
        }
        private async Task<string> UploadFileToBlobAsync(IFormFile file, string containerName, IConfiguration configuration)
        {
            // Upload a blob with .NET
            // source = https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blob-upload
            // used to understand more about how to upload file to blob storage. also applied knowledge from 2nd year.

            var connectionString = configuration.GetValue<string>("ConnectionStrings:StorageConnectionString");

            var blobServiceClient = new BlobServiceClient(connectionString);

            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await blobContainerClient.CreateIfNotExistsAsync();

            // Azure blob storage - auto generate unique blob name
            // link = https://stackoverflow.com/questions/14319340/azure-blob-storage-auto-generate-unique-blob-name
            // author = Sandrino Di Mattia
            // author link = https://stackoverflow.com/users/384546/sandrino-di-mattia
            // learned how to make unique names with GUID

            var blobName = $"{Guid.NewGuid()}-{file.FileName}";
            var blobClient = blobContainerClient.GetBlobClient(blobName);
            var contentType = file.ContentType;

            // BlobHttpHeaders Class
            // source = https://learn.microsoft.com/en-us/dotnet/api/azure.storage.blobs.models.blobhttpheaders?view=azure-dotnet
            // how to set it so that the user opens the image in brower with content disposition set to inline, this prevents the user from downloading the image.

            var headers = new BlobHttpHeaders
            {
                ContentType = contentType,
                ContentDisposition = "inline"
            };

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, headers);
            }

            return blobClient.Uri.ToString();
        }
        private async Task DeleteBlobAsync(string blobUrl, IConfiguration configuration)
        {
            try
            {
                var connectionString = configuration.GetValue<string>("ConnectionStrings:StorageConnectionString");

                Uri uri = new Uri(blobUrl);

                // Get last path from URL
                // link = https://stackoverflow.com/questions/54968854/get-last-path-from-url
                // author = YosiFZ
                // author link = https://stackoverflow.com/users/679099/yosifz
                // learned how to get the name of the blob from the last part of the uri

                string blobName = uri.Segments.Last();
                string containerName = "animals";

                // get rid of escape characters from uri blob name
                // link = https://stackoverflow.com/questions/239567/decode-escaped-url-without-using-httputility-urldecode
                // author = Igal Tabachnik
                // author link = https://stackoverflow.com/users/8205/igal-tabachnik
                // learned how to get rid of escape characters from uri blob name

                blobName = Uri.UnescapeDataString(blobName);

                var blobServiceClient = new BlobServiceClient(connectionString);

                var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = blobContainerClient.GetBlobClient(blobName);

                var exists = await blobClient.ExistsAsync();
                await blobClient.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting blob: {ex.Message}");
                throw;
            }
        }
        // created jwt token
        private string GenerateJwtToken(User user)
        {
            // Adding JWT Authentication & Authorization in ASP.NET Core
            // link = https://www.youtube.com/watch?v=mgeuh8k3I4g&t=306s
            // author = Nick Chapsas
            // learned how to use jwts in c#
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserEmail),
                new Claim("UserType", user.UserType),
                new Claim("UserEmail", user.UserEmail),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-very-secure-key-ThisIsTheSPCAKeyDontTellAnyOne"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "SPCA",
                audience: "SPCA",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
