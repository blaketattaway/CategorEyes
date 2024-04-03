using Radzen;

namespace OneCore.CategorEyes.Client.Models.Requests
{
    public class ReportRequest
    {
        public List<string> Headers { get; set; }
        public string? Filter { get; set; }
        public SortDescriptor? Sort { get; set; }
    }
}
