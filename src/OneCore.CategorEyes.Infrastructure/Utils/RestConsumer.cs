using Newtonsoft.Json;
using OneCore.CategorEyes.Commons.Consts;
using OneCore.CategorEyes.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCore.CategorEyes.Infrastructure.Utils
{
    internal sealed class RestConsumer
    {
        private static readonly int _timeOut;
        private static readonly string _openAIAPIURL = ConfigurationHelper.Value("OpenAIAPIURL");
        private static readonly HttpClient? _openAIAPIClient;
        private static readonly string _openAIAPIKey = ConfigurationHelper.Value("OpenAIAPIKey");
        private readonly HttpClient? _client = null;

        static RestConsumer()
        {

            if (!int.TryParse(ConfigurationHelper.Value("timeout"), out _timeOut))
            {
                _timeOut = 5;
            }

            if (!string.IsNullOrEmpty(_openAIAPIURL))
            {
                _openAIAPIClient = new HttpClient
                {
                    BaseAddress = new(_openAIAPIURL!),
                    Timeout = TimeSpan.FromMinutes(_timeOut)
                };

                if (!string.IsNullOrEmpty(_openAIAPIKey))
                {
                    _openAIAPIClient.DefaultRequestHeaders.Add(AuthHeaderNames.AUTHORIZATION, $"Bearer {_openAIAPIKey}");
                }
            }
        }

        public RestConsumer(BaseAPI baseAPI)
        {
            switch (baseAPI)
            {
                case BaseAPI.OpenAIAPI:
                    _client = _openAIAPIClient;
                    break;
            }
        }

        /// <summary>
        /// Hace peticiones post al endpoint enviado con un objeto enviado
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="url"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<T?> PostResponse<T, U>(string url, U obj)
        {
            T? ret = default;
            HttpResponseMessage? response = null;

            await RetryHelper.Execute(
                async () =>
                {
                    response = await _client!.PostAsync(url, CreateHttpContent(obj));
                    response?.EnsureSuccessStatusCode();
                });

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
            HttpResponseMessage? response = null;

            await RetryHelper.Execute(
                async () =>
                {
                    response = await _client!.GetAsync(url);
                    response?.EnsureSuccessStatusCode();
                });

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
