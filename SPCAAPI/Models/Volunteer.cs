using System;
using System.Collections.Generic;

namespace SPCAAPI.Models;

public partial class Volunteer
{
    public int VolunteerId { get; set; }

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string VolunteerDate { get; set; } = null!;

    public string Name { get; set; } = null!;
}
