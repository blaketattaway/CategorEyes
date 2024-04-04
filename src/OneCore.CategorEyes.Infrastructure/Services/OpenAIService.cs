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

        /// <summary>
        /// Initializes a new instance of the OpenAIService class using the provided application configuration to configure an HttpClientFactory.
        /// </summary>
        /// <param name="configuration">The application configuration containing settings for HTTP clients, of type <see cref="IConfiguration"/>.</param>
        public OpenAIService(IConfiguration configuration)
        {
            _httpClientFactory = new HttpClientFactory(configuration);
        }

        /// <inheritdoc />
        public async Task<OpenAIAnalysisResponse> Analyze(object request)
        {
            return await new RestConsumer(_httpClientFactory, BaseAPI.OpenAIAPI).PostResponseAsync<OpenAIAnalysisResponse, object>("chat/completions", request);
        }
    }
}
