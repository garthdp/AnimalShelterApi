namespace SPCAAPI.Models
{
    public class BoardingRequest
    {
        public string PetName { get; set; }
        public string Breed { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
    }
}
