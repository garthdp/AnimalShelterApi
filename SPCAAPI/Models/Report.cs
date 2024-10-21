namespace SPCAAPI.Models
{
    public class Report
    {
        public int Id { get; set; } 
        public string Location { get; set; }
        public string Description { get; set; }
        public string ContactInfo { get; set; } 
        public string Status { get; set; } 
        public DateTime ReportDate { get; set; } 
    }
}
