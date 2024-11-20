using System;
using System.Collections.Generic;

namespace SPCAAPI.Models;

public partial class Report
{
    public int ReportId { get; set; }

    public string ContactInfo { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Location { get; set; } = null!;

    public string Status { get; set; } = null!;
}
