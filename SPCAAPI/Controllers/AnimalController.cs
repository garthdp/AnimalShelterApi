using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using Firebase.Storage;
using SPCAAPI.Models;
using SPCAAPI.Data;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs.Models;

namespace SPCAAPI.Controllers
{
    [Route("api/Animal")]
    [ApiController]
    public class AnimalController : Controller
    {
        public static FirestoreDb db = establishCon();
        private readonly WilDbContext _context;
        private readonly IConfiguration _configuration;
        public AnimalController(WilDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public static FirestoreDb establishCon()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"wilspca.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            FirestoreDb db = FirestoreDb.Create("wilspca");
            return db;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] RecieveAnimal animal)
        {
            /*
            Code Attribution
            Title: C# Firebase Tutorial | Firebase Storage Upload Files in .NET Core Web API
            Author: Programming Guru
            Link: https://www.youtube.com/watch?v=nh17WlHtODs
            Usage: Used to understand how to upload files to Firebase Storage using .NET Core
            */

            if (animal.file == null || animal.file.Length == 0)
            {
                return BadRequest(new { message = "File is required." });
            }
            try
            {
                // Upload the file to Azure Blob Storage and get the URL
                var fileUrl = await UploadFileToBlobAsync(animal.file, "animals", _configuration);

                Animal saveAnimal = new Animal();
                saveAnimal.AdoptionStatus = animal.AdoptionStatus;
                saveAnimal.Weight = int.Parse(animal.Weight);
                saveAnimal.Breed = animal.Breed;
                saveAnimal.Health = animal.Health;
                saveAnimal.Name = animal.Name;
                saveAnimal.AnimalType = animal.AnimalType;
                saveAnimal.ImageUrl = fileUrl;

                // Save the animal data to the database
                _context.Animals.Add(saveAnimal);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Animal added successfully", fileUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error uploading file", error = ex.Message });
            }
        }

        [HttpGet("GetAnimals")]
        public async Task<IActionResult> GetAnimals()
        {
            /*
            Code Attribution
            Title: C# Firestore Tutorial 3 | How to Retrieve Data | GET Data | English
            Author: The Amazing Codeverse
            Link: https://www.youtube.com/watch?v=SrRrxYBR3s0&list=PLrb70iTVZjZPEbhCh85VQIpRbQos2Qx3i&index=3
            Usage: Used to get collection of data from Firestore database 
            */
            var animals = _context.Animals.ToList();

            if (animals == null)
            {
                return NotFound(new { message = "No animals found." });
            }

            return Ok(animals);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var animal = _context.Animals.Where(x => x.AnimalId == id).FirstOrDefault();

            if (animal != null)
            {
                try
                {
                    await DeleteBlobAsync(animal.ImageUrl, _configuration);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = $"Error deleting image/video from Blob Storage: {ex.Message}" });
                }

                _context.Animals.Remove(animal);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Animal and associated image deleted" });
            }
            else
            {
                return NotFound(new { message = "Animal not found" });
            }
        }
        [HttpPatch]
        public async Task<IActionResult> Patch(int id, [FromForm] RecieveAnimal animal)
        {
            var foundAnimal = _context.Animals.Where(x => x.AnimalId == id).FirstOrDefault();

            if (foundAnimal == null)
            {
                return NotFound(new { message = "Animal not found" });
            }

            if (animal.file != null && animal.file.Length > 0)
            {
                if (!string.IsNullOrEmpty(foundAnimal.ImageUrl))
                {
                    try
                    {
                        await DeleteBlobAsync(foundAnimal.ImageUrl, _configuration);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new { message = $"Error deleting old image: {ex.Message}" });
                    }
                }
                try
                {
                    var newImageUrl = await UploadFileToBlobAsync(animal.file, "animals", _configuration);
                    foundAnimal.ImageUrl = newImageUrl;
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = $"Error uploading new image: {ex.Message}" });
                }
            }

            if (!string.IsNullOrEmpty(animal.Name))
            {
                foundAnimal.Name = animal.Name;
            }
            if (!string.IsNullOrEmpty(animal.Breed))
            {
                foundAnimal.Breed = animal.Breed;
            }
            if (!string.IsNullOrEmpty(animal.Health))
            {
                foundAnimal.Health = animal.Health;
            }
            if (int.Parse(animal.Weight) != 0)
            {
                foundAnimal.Weight = int.Parse(animal.Weight);
            }
            if (!string.IsNullOrEmpty(animal.AnimalType))
            {
                foundAnimal.AnimalType = animal.AnimalType;
            }
            if (!string.IsNullOrEmpty(animal.AdoptionStatus))
            {
                foundAnimal.AdoptionStatus = animal.AdoptionStatus;
            }
            _context.Animals.Update(foundAnimal);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Animal updated", foundAnimal });
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
    }
}
