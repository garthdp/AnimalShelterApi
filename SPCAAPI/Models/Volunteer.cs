using Microsoft.AspNetCore.Mvc;

namespace SPCAAPI.Models
{
    public class Volunteer
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string VolunteerDate { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

    }
}
