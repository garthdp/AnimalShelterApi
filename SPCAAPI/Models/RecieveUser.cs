namespace SPCAAPI.Models
{
    public class RecieveUser
    {
        public string? UserEmail { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserType { get; set; }
        public string? ProfilePicture { get; set; }
        public IFormFile? file { get; set; }
    }
}
