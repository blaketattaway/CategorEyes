using OneCore.CategorEyes.Commons.Entities;

namespace OneCore.CategorEyes.Commons.Responses
{
    public class LogResponse
    {
        public List<Historical> Historicals { get; set; }
        public int TotalPages { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
