namespace OneCore.CategorEyes.Client.Models.Responses
{
    public class LogResponse
    {
        public List<Historical> Historicals { get; set; } 
        public int TotalPages { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
