using Microsoft.Extensions.Configuration;
using OneCore.CategorEyes.Commons.Consts;
using System.Net.Http.Headers;

namespace OneCore.CategorEyes.Infrastructure.Utils
{
    internal sealed class HttpClientFactory
    {
        private readonly IConfiguration _configuration;

        public HttpClientFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public HttpClient CreateClient(BaseAPI baseAPI)
        {
            HttpClient client = new HttpClient();

            switch (baseAPI)
            {
                case BaseAPI.OpenAIAPI:
                    string openAIAPIURL = _configuration["OpenAIAPIURL"];
                    string openAIAPIKey = _configuration["OpenAIAPIKey"];
                    int timeout = int.TryParse(_configuration["timeout"], out int parsedTimeout) ? parsedTimeout : 5;

                    client.BaseAddress = new Uri(openAIAPIURL);
                    client.Timeout = TimeSpan.FromMinutes(timeout);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", openAIAPIKey);
                    break;
            }

            return client;
        }
    }
}
