using System;
using System.Collections.Generic;

namespace SPCAAPI.Models;

public partial class User
{
    public string UserEmail { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string City { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string UserType { get; set; } = null!;

    public string ProfilePicture { get; set; } = null!;
}
