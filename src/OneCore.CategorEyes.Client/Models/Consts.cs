namespace OneCore.CategorEyes.Client.Models
{
    public class Consts
    {
        public enum BaseAPI
        { 
            CategorEyes
        }

        public enum FileType
        {
            Image,
            Pdf,
            Unknown
        }

        public enum DocumentType
        {
            Invoice,
            GeneralText
        }

        public static class URLs
        {
            public const string CATEGOREYES_API = "https://localhost:7112/api/";
        }

        public static class ContentType
        {
            public const string APPLICATION_JSON = "application/json";
        }

        public static class Endpoints
        {
            public const string ANALYSIS = "Analysis/Analyze";
            public const string GET_LOGS = "Log/GetLogs";
        }
    }
}
