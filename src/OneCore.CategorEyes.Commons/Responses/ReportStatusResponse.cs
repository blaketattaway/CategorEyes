namespace OneCore.CategorEyes.Commons.Responses
{
    public class ReportStatusResponse
    {
        public string Guid { get; set; }
        public bool Ready { get; set; }
        public string Url { get; set; }
        public string Progress { get; set; }
        public bool HasError { get; set; }
        public string ErrorDescription { get; set; }
        //public ReportType ReportType { get; set; }
    }
}
