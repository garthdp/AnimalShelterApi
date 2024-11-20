using System;
using System.Collections.Generic;

namespace SPCAAPI.Models;

public partial class Animal
{
    public int AnimalId { get; set; }

    public string AdoptionStatus { get; set; } = null!;

    public string AnimalType { get; set; } = null!;

    public string Breed { get; set; } = null!;

    public string Health { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int Weight { get; set; }
}
