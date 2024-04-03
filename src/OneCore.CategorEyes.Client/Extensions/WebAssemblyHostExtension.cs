using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Globalization;

namespace OneCore.CategorEyes.Client.Extensions
{
    public static class WebAssemblyHostExtension
    {
        private const string DEFAULT_CULTURE = "en-US";

        public async static Task SetDefaultCulture(this WebAssemblyHost host)
        {
            var jsInterop = host.Services.GetRequiredService<IJSRuntime>();

            var result = await SafeInvokeAsync(jsInterop, "blazorCulture.get", DEFAULT_CULTURE);

            var culture = new CultureInfo(result);

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

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
