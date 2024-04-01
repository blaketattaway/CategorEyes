using OneCore.CategorEyes.Commons.Consts;

namespace OneCore.CategorEyes.Commons.Responses
{
    public class AnalysisResponse
    {
        public string DocumentTypeName { get; set; }
        public DocumentType DocumentType { get; set; }
        public string Data { get; set; }
        public string AdditionalData { get; set; }
        public string FileName { get; set; }
    }
}
