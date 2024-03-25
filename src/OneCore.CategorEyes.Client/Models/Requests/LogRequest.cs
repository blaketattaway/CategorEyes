namespace OneCore.CategorEyes.Client.Models.Requests
{
    public class LogRequest
    {
        public int PageSize { get; set; } = 10;
        public int Page { get; set; } = 1;
        public string? Filter { get; set; }
    }
}
