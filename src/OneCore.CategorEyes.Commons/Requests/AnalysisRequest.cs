using OneCore.CategorEyes.Commons.Consts;

namespace OneCore.CategorEyes.Commons.Requests
{
    public class AnalysisRequest
    {
        public string Base64File { get; set; }
        public FileType FileType { get; set; }
        public string FileTypeName { get; set; }
    }
}
