using System;
using System.Collections.Generic;

namespace SPCAAPI.Models;

public partial class BoardingRequest
{
    public string Breed { get; set; } = null!;

    public string EndDate { get; set; } = null!;

    public string OwnerName { get; set; } = null!;

    public string OwnerEmail { get; set; } = null!;

    public string PetName { get; set; } = null!;

    public string StartDate { get; set; } = null!;

    public int BoardingId { get; set; }
}
