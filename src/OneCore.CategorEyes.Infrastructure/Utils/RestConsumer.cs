using Newtonsoft.Json;
using OneCore.CategorEyes.Commons.Consts;
using OneCore.CategorEyes.Infrastructure.Helpers;
using System.Text;

namespace OneCore.CategorEyes.Infrastructure.Utils
{
    internal sealed class RestConsumer
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Initializes a new instance of the RestConsumer class using an HttpClient provided by the HttpClientFactory based on the specified API type.
        /// </summary>
        /// <param name="httpClientFactory">An instance of HttpClientFactory used to create the HttpClient.</param>
        /// <param name="baseAPI">The BaseAPI enumeration indicating the type of API for which the RestConsumer is being initialized.</param>
        public RestConsumer(HttpClientFactory httpClientFactory, BaseAPI baseAPI)
        {
            _client = httpClientFactory.CreateClient(baseAPI);
        }

        /// <summary>
        /// Asynchronously sends a POST request to the specified URL with the provided object serialized as JSON in the request body, and deserializes the response to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to which the response will be deserialized, specified as a generic type parameter.</typeparam>
        /// <typeparam name="U">The type of the object being sent in the request body, specified as a generic type parameter.</typeparam>
        /// <param name="url">The URL to which the POST request is sent, of type <see cref="string"/>.</param>
        /// <param name="obj">The object to be sent in the request body, of type U.</param>
        /// <returns>A task representing the asynchronous operation, returning the deserialized response of type T, or null if the request was not successful, of type <see cref="Task{T?}"/>.</returns>
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

        /// <summary>
        /// Asynchronously sends a GET request to the specified URL and deserializes the response to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to which the response will be deserialized, specified as a generic type parameter.</typeparam>
        /// <param name="url">The URL to which the GET request is sent, of type <see cref="string"/>.</param>
        /// <returns>A task representing the asynchronous operation, returning the deserialized response of type T, or null if the request was not successful, of type <see cref="Task{T?}"/>.</returns>
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

        /// <summary>
        /// Serializes the provided object to JSON and encapsulates it in an HttpContent object suitable for use in an HTTP request.
        /// </summary>
        /// <typeparam name="T">The type of the object to be serialized, specified as a generic type parameter.</typeparam>
        /// <param name="content">The object to be serialized to JSON, of type T.</param>
        /// <returns>An HttpContent object containing the serialized JSON representation of the provided object, of type <see cref="HttpContent"/>.</returns>
        private static HttpContent CreateHttpContent<T>(T content)
        {
            var json = JsonConvert.SerializeObject(content);
            return new StringContent(json, Encoding.UTF8, ContentType.APPLICATION_JSON);
        }
    }
}
