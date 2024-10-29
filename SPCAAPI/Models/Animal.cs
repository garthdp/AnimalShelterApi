namespace SPCAAPI.Models
{
    public class Animal
    {
        public string? Name { get; set; }
        public string? Breed { get; set; }
        public string? Health { get; set; }
        public string? Weight { get; set; }
        public string? AdoptionStatus { get; set; }
        public string? AnimalType { get; set; }
        public IFormFile? Image { get; set; }
    }
}
