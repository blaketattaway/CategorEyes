using OneCore.CategorEyes.Commons.Responses;

namespace OneCore.CategorEyes.Business.Services
{
    public interface IOpenAIService
    {
        Task<OpenAIAnalysisResponse> Analyze(object request);
    }
}
