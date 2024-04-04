using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Globalization;

namespace OneCore.CategorEyes.Client.Extensions
{
    public static class WebAssemblyHostExtension
    {
        private const string DEFAULT_CULTURE = "en-US";

        /// <summary>
        /// Sets the default culture for the application based on a persisted value or falls back to a default if not set.
        /// </summary>
        /// <param name="host">The <see cref="WebAssemblyHost"/> instance to extend, providing access to application services.</param>
        /// <returns>A task that represents the asynchronous operation of setting the application's default culture.</returns>
        public async static Task SetDefaultCulture(this WebAssemblyHost host)
        {
            var jsInterop = host.Services.GetRequiredService<IJSRuntime>();

            var result = await SafeInvokeAsync(jsInterop, "blazorCulture.get", DEFAULT_CULTURE);

            var culture = new CultureInfo(result);

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        /// <summary>
        /// Safely invokes a JavaScript function and returns the result. If the invocation fails, returns a default value.
        /// </summary>
        /// <param name="jsInterop">The <see cref="IJSRuntime"/> instance used for JavaScript interop calls.</param>
        /// <param name="method">The JavaScript function to invoke.</param>
        /// <param name="defaultValue">The default value to return in case the JavaScript function invocation fails.</param>
        /// <returns>A task that represents the asynchronous operation, containing the result of the JavaScript function invocation or the default value if the invocation fails.</returns>
        private static async Task<string> SafeInvokeAsync(IJSRuntime jsInterop, string method, string defaultValue)
        {
            try
            {
                return await jsInterop.InvokeAsync<string>(method) ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
