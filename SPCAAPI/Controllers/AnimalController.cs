using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using Firebase.Storage;
using SPCAAPI.Models;

namespace SPCAAPI.Controllers
{
    [Route("api/Animal")]
    [ApiController]
    public class AnimalController : Controller
    {
        public static FirestoreDb db = establishCon();
        public static FirestoreDb establishCon()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"wilspca.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            FirestoreDb db = FirestoreDb.Create("wilspca");
            return db;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] Animal animal)
        {
            /*
            Code Attribution
            Title: C# Firebase Tutorial | Firebase Storage Upload Files in .NET Core Web API
            Author: Programming Guru
            Link: https://www.youtube.com/watch?v=nh17WlHtODs
            Usage: Used to understand how to upload files to Firebase Storage using .NET Core
            */

            string imageUrl = null;
            if (animal.Image != null && animal.Image.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(animal.Image.FileName);

                var stream = animal.Image.OpenReadStream();
                var firebaseStorage = new FirebaseStorage(
                    "wilspca.appspot.com");

                var uploadTask = firebaseStorage
                    .Child("animal_images") 
                    .Child(fileName)
                    .PutAsync(stream);

                imageUrl = await uploadTask;
            }

            CollectionReference coll = db.Collection("Animals");
            DocumentReference docRef = coll.Document();

            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "name", animal.Name },
                { "breed", animal.Breed },
                { "health", animal.Health },
                { "weight", animal.Weight },
                { "adoptionStatus", animal.AdoptionStatus },
                { "imageUrl", imageUrl } 
            };

            await docRef.SetAsync(data);

            return Ok(new { message = "Added animal", imageUrl });
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
            Query qRef = db.Collection("Animals");
            QuerySnapshot snapshot = await qRef.GetSnapshotAsync();

            if (snapshot == null)
            {
                return NotFound(new { message = "No animals found." });
            }

            List<Dictionary<string, object>> animals = new List<Dictionary<string, object>>();

            foreach (DocumentSnapshot docsnap in snapshot)
            {
                Dictionary<string, object> entry = docsnap.ConvertTo<Dictionary<string, object>>();
                entry.Add("petId", docsnap.Reference.Id.ToString());

                if (docsnap.Exists)
                {
                    animals.Add(entry);
                }
            }

            return Ok(animals);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            DocumentReference docref = db.Collection("Animals").Document(id);
            DocumentSnapshot snapshot = await docref.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                string imageUrl = snapshot.GetValue<string>("imageUrl");

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    // format the string to get to the firebase folder
                    var imagePath = imageUrl.Substring(imageUrl.IndexOf("o/") + 2);
                    imagePath = imagePath.Substring(0, imagePath.IndexOf("?alt="));

                    // replace %2f in string to / to make sure formating is correct
                    imagePath = imagePath.Replace("%2F", "/");

                    var firebaseStorage = new FirebaseStorage("wilspca.appspot.com");
                    try
                    {
                        await firebaseStorage
                            .Child(imagePath)
                            .DeleteAsync();
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new { message = $"Error deleting image: {ex.Message}, {imagePath}" });
                    }
                }

                // Delete the document from Firestore
                await docref.DeleteAsync();
                return Ok(new { message = "Animal and associated image deleted" });
            }
            else
            {
                return NotFound(new { message = "Animal not found" });
            }
        }
        [HttpPatch]
        public async Task<IActionResult> Patch(string id, [FromForm] Animal animal)
        {
            DocumentReference docRef = db.Collection("Animals").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                return NotFound(new { message = "Animal not found" });
            }

            Dictionary<string, object> updates = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(animal.Name))
            {
                updates["name"] = animal.Name;
            }
            if (!string.IsNullOrEmpty(animal.Breed))
            {
                updates["breed"] = animal.Breed;
            }
            if (!string.IsNullOrEmpty(animal.Health))
            {
                updates["health"] = animal.Health;
            }
            if (!string.IsNullOrEmpty(animal.Weight))
            {
                updates["weight"] = animal.Weight;
            }
            if (!string.IsNullOrEmpty(animal.AdoptionStatus))
            {
                updates["adoptionStatus"] = animal.AdoptionStatus;
            }
            string newImageUrl = null;
            if (animal.Image != null && animal.Image.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(animal.Image.FileName);

                var stream = animal.Image.OpenReadStream();
                var firebaseStorage = new FirebaseStorage(
                    "wilspca.appspot.com");

                var uploadTask = firebaseStorage
                    .Child("animal_images")
                    .Child(fileName)
                    .PutAsync(stream);

                newImageUrl = await uploadTask;
                updates["imageUrl"] = newImageUrl;

                string oldImageUrl = snapshot.GetValue<string>("imageUrl");

                if (!string.IsNullOrEmpty(oldImageUrl))
                {
                    // format the string to get to the firebase folder
                    var imagePath = oldImageUrl.Substring(oldImageUrl.IndexOf("o/") + 2);
                    imagePath = imagePath.Substring(0, imagePath.IndexOf("?alt="));

                    // replace %2f in string to / to make sure formating is correct
                    imagePath = imagePath.Replace("%2F", "/");

                    try
                    {
                        await firebaseStorage
                            .Child(imagePath)
                            .DeleteAsync();
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new { message = $"Error deleting image: {ex.Message}, {imagePath}" });
                    }
                }
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
