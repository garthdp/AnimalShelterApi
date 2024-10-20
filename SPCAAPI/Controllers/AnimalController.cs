using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using Firebase.Storage;

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
        public async Task<IActionResult> Post(string name, string breed, string health, string weight, string adoptionStatus, IFormFile image)
        {
            /*
            Code Attribution
            Title: C# Firebase Tutorial | Firebase Storage Upload Files in .NET Core Web API
            Author: Programming Guru
            Link: https://www.youtube.com/watch?v=nh17WlHtODs
            Usage: Used to understand how to upload files to Firebase Storage using .NET Core
            */

            string imageUrl = null;
            if (image != null && image.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

                var stream = image.OpenReadStream();
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
                { "name", name },
                { "breed", breed },
                { "health", health },
                { "weight", weight },
                { "adoptionStatus", adoptionStatus },
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
                await docref.DeleteAsync();
            }
            else
            {
                return NotFound(new { message = "Animal not found" });
            }
            return Ok(new { message = "Animal deleted" });
        }
        [HttpPatch]
        public async Task<IActionResult> Patch(string id, string name = null, string breed = null, string health = null, string weight = null, string adoptionStatus = null)
        {
            DocumentReference docRef = db.Collection("Animals").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                return NotFound(new { message = "Animal not found" });
            }

            Dictionary<string, object> updates = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(name))
            {
                updates["name"] = name;
            }
            if (!string.IsNullOrEmpty(breed))
            {
                updates["breed"] = breed;
            }
            if (!string.IsNullOrEmpty(health))
            {
                updates["health"] = health;
            }
            if (!string.IsNullOrEmpty(weight))
            {
                updates["weight"] = weight;
            }
            if (!string.IsNullOrEmpty(adoptionStatus))
            {
                updates["adoptionStatus"] = adoptionStatus;
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
