using OneCore.CategorEyes.Commons.Consts;

namespace OneCore.CategorEyes.Commons.Requests
{
    public class LogRequest
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public string? Filter { get; set; }
        public SortDescriptor? Sort { get; set; }
    }

    public class SortDescriptor
    {
        public string Property { get; set; }
        public SortOrder? SortOrder { get; set; }
    }
}
