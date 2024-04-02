using Newtonsoft.Json;
using OneCore.CategorEyes.Commons.Consts;
using OneCore.CategorEyes.Infrastructure.Helpers;
using System.Text;

namespace OneCore.CategorEyes.Infrastructure.Utils
{
    internal sealed class RestConsumer
    {
        private readonly HttpClient _client;

        public RestConsumer(HttpClientFactory httpClientFactory, BaseAPI baseAPI)
        {
            _client = httpClientFactory.CreateClient(baseAPI);
        }

        public async Task<T?> PostResponseAsync<T, U>(string url, U obj)
        {
            HttpResponseMessage response = await RetryHelper.Execute(() => _client.PostAsync(url, CreateHttpContent(obj)));

            if (response.IsSuccessStatusCode)
            {
                string resp = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(resp);
            }

            return default;
        }

        public async Task<T?> GetResponseAsync<T>(string url)
        {
            HttpResponseMessage response = await RetryHelper.Execute(() => _client.GetAsync(url));

            if (response.IsSuccessStatusCode)
            {
                string resp = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(resp);
            }

            return default;
        }

        private static HttpContent CreateHttpContent<T>(T content)
        {
            var json = JsonConvert.SerializeObject(content);
            return new StringContent(json, Encoding.UTF8, ContentType.APPLICATION_JSON);
        }
    }
}
