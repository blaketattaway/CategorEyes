using Microsoft.Extensions.Configuration;
using OneCore.CategorEyes.Business.Services;
using OneCore.CategorEyes.Commons.Consts;
using OneCore.CategorEyes.Commons.Responses;
using OneCore.CategorEyes.Infrastructure.Utils;

namespace OneCore.CategorEyes.Infrastructure.Services
{
    internal class OpenAIService : IOpenAIService
    {
        private readonly HttpClientFactory _httpClientFactory;

        public OpenAIService(IConfiguration configuration)
        {
            _httpClientFactory = new HttpClientFactory(configuration);
        }

        public async Task<OpenAIAnalysisResponse> Analyze(object request)
        {
            return await new RestConsumer(_httpClientFactory, BaseAPI.OpenAIAPI).PostResponseAsync<OpenAIAnalysisResponse, object>("chat/completions", request);
        }
    }
}
