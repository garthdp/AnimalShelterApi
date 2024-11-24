namespace SPCAAPI.Models
{
    public class RecieveAnimal
    {
        public int AnimalId { get; set; }

        public string AdoptionStatus { get; set; } = null!;

        public string AnimalType { get; set; } = null!;

        public string Breed { get; set; } = null!;

        public string Health { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Weight { get; set; } = null!;
        public IFormFile file { get; set; }
    }
}
