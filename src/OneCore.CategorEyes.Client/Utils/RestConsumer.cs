using System.Net.Http.Json;

namespace OneCore.CategorEyes.Client.Utils
{
    internal sealed class RestConsumer
    {
        private readonly HttpClient _client;

        public RestConsumer(HttpClient client)
        {
            _client = client;
        }

        public async Task<T?> PostResponse<T, U>(string url, U obj)
        {
            var response = await _client.PostAsJsonAsync(url, obj);
            response.EnsureSuccessStatusCode();

            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<T>() : default;
        }

        public async Task<T?> GetResponse<T>(string url)
        {
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<T>() : default;
        }
    }
}
