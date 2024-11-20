using System;
using System.Collections.Generic;

namespace SPCAAPI.Models;

public partial class Event
{
    public int EventId { get; set; }

    public string EventDate { get; set; } = null!;

    public string EventDescription { get; set; } = null!;

    public string EventName { get; set; } = null!;
}
