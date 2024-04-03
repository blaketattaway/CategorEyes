namespace OneCore.CategorEyes.Commons.Requests
{
    public class ReportRequest
    {
        public List<string> Headers { get; set; }
        public string? Filter { get; set; }
        public SortDescriptor? Sort { get; set; }
    }
}
