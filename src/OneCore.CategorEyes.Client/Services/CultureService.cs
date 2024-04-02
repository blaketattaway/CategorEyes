using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Globalization;

namespace OneCore.CategorEyes.Client.Services
{
    public class CultureService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly NavigationManager _navigationManager;

        public CultureService(IJSRuntime jsRuntime, NavigationManager navigationManager)
        {
            _jsRuntime = jsRuntime;
            _navigationManager = navigationManager;
        }

        /// <summary>
        /// Sets the application's culture to the specified new culture and forces the application to reload.
        /// </summary>
        /// <param name="newCulture">The new culture to set for the application, of type <see cref="CultureInfo"/>.</param>
        public void SetCulture(CultureInfo newCulture)
        {
            if (CultureInfo.CurrentCulture != newCulture)
            {
                var js = (IJSInProcessRuntime)_jsRuntime;
                js.InvokeVoid("blazorCulture.set", newCulture.Name);

                _navigationManager.NavigateTo(_navigationManager.Uri, forceLoad: true);
            }
        }
    }
}
