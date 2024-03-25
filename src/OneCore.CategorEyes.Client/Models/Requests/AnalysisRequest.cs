using static OneCore.CategorEyes.Client.Models.Consts;

namespace OneCore.CategorEyes.Client.Models.Requests
{
    public class AnalysisRequest
    {
        public string Base64File { get; set; }
        public string FileTypeName { get; set; }
        public FileType FileType { get; set; }
    }
}
