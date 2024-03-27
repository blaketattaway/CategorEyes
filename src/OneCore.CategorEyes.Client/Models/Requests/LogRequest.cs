using Radzen;

namespace OneCore.CategorEyes.Client.Models.Requests
{
    public class LogRequest
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public string? Filter { get; set; }
        public SortDescriptor? Sort { get; set; }
    }
}
