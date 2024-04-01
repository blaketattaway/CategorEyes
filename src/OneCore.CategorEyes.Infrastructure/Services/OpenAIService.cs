using OneCore.CategorEyes.Business.Services;
using OneCore.CategorEyes.Commons.Consts;
using OneCore.CategorEyes.Commons.Responses;
using OneCore.CategorEyes.Infrastructure.Utils;

namespace OneCore.CategorEyes.Infrastructure.Services
{
    internal class OpenAIService : IOpenAIService
    {
        public async Task<OpenAIAnalysisResponse> Analyze(object request)
        {
            return await new RestConsumer(BaseAPI.OpenAIAPI).PostResponse<OpenAIAnalysisResponse, object>("chat/completions", request);
        }
    }
}
