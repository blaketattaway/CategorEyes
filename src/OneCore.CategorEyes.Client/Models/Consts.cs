using System.ComponentModel.DataAnnotations;

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
            GeneralText,
            Other
        }

        public enum HistoricalType
        {
            [Display(Name = "Document Upload")]
            DocumentUpload = 1,
            [Display(Name = "IA")]
            IA = 2,
            [Display(Name = "User Interaction")]
            UserInteraction = 3,
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
