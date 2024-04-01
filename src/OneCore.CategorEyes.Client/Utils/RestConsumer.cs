using Newtonsoft.Json;
using System.Text;
using static OneCore.CategorEyes.Client.Models.Consts;

namespace OneCore.CategorEyes.Client.Utils
{
    internal sealed class RestConsumer
    {
        private static readonly int _timeout = 5;
        private static readonly string _categorEyesAPIURL = URLs.CATEGOREYES_API;
        private static HttpClient? _categorEyesAPIClient;
        private static HttpClient? _client;

        static RestConsumer()
        {
            if (!string.IsNullOrEmpty(_categorEyesAPIURL))
            {
                _categorEyesAPIClient = new HttpClient
                {
                    BaseAddress = new Uri(_categorEyesAPIURL),
                    Timeout = TimeSpan.FromMinutes(_timeout)
                };
            }
        }

        public RestConsumer(BaseAPI baseAPI)
        {
            switch (baseAPI)
            {
                case BaseAPI.CategorEyes:
                    _client = _categorEyesAPIClient;
                    break;
            }
        }

        public async Task<T?> PostResponse<T, U>(string url, U obj)
        {
            T? ret = default;
            HttpResponseMessage? response = await _client!.PostAsync(url, CreateHttpContent(obj));
            response?.EnsureSuccessStatusCode();

            if (response?.IsSuccessStatusCode ?? false)
            {
                string resp = await response!.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(resp);
            }

            return ret;
        }

        /// <summary>
        /// Hace peticiones get al endpoint enviado
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<T?> GetResponse<T>(string url)
        {
            T? ret = default;
            HttpResponseMessage? response = await _client!.GetAsync(url);
            response?.EnsureSuccessStatusCode();

            if (response?.IsSuccessStatusCode ?? false)
            {
                string resp = await response!.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(resp);
            }

            return ret;
        }

        /// <summary>
        /// Convierte un objeto de C# a un JSON
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        private static HttpContent CreateHttpContent<T>(T content)
        {
            var json = JsonConvert.SerializeObject(content);
            return new StringContent(json, Encoding.UTF8, ContentType.APPLICATION_JSON);
        }
    }
}
