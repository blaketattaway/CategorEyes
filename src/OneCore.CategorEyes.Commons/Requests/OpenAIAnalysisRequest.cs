using OneCore.CategorEyes.Commons.OpenAI;

namespace OneCore.CategorEyes.Commons.Requests
{
    public class OpenAIAnalysisRequest
    {
        public string model { get; set; }
        public List<Message> messages { get; set; }
        public int max_tokens { get; set; } = 1000;
    }
}
