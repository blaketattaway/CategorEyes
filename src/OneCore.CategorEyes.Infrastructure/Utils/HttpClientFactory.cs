using Microsoft.Extensions.Configuration;
using OneCore.CategorEyes.Commons.Consts;
using System.Net.Http.Headers;

namespace OneCore.CategorEyes.Infrastructure.Utils
{
    internal sealed class HttpClientFactory
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the HttpClientFactory class using the provided application configuration.
        /// </summary>
        /// <param name="configuration">The application configuration containing settings for HTTP clients, of type <see cref="IConfiguration"/>.</param>
        public HttpClientFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Creates and configures an HttpClient instance based on the specified BaseAPI enumeration. This includes setting the base address, timeout, and authorization header as per the API requirements.
        /// </summary>
        /// <param name="baseAPI">The BaseAPI enumeration indicating the type of API for which the HttpClient is being created, of type <see cref="BaseAPI"/>.</param>
        /// <returns>A configured HttpClient instance for interacting with the specified API, of type <see cref="HttpClient"/>.</returns>
        public HttpClient CreateClient(BaseAPI baseAPI)
        {
            HttpClient client = new();

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
