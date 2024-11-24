namespace SPCAAPI.Models
{
    public class RecieveAnimal
    {
        public int AnimalId { get; set; }

        public string? AdoptionStatus { get; set; }

        public string? AnimalType { get; set; }

        public string? Breed { get; set; }

        public string? Health { get; set; }

        public string? ImageUrl { get; set; }

        public string? Name { get; set; }

        public string? Weight { get; set; }
        public IFormFile? file { get; set; }
    }
}
