using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OneCore.CategorEyes.Client;
using OneCore.CategorEyes.Client.Extensions;
using OneCore.CategorEyes.Client.Services;
using OneCore.CategorEyes.Client.Utils;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddRadzenComponents();
builder.Services.AddLocalization();
builder.Services.AddScoped<CultureService>();
builder.Services.AddScoped<RestConsumer>();
builder.Services.AddScoped<LoadingService>();

var host = builder.Build();

await host.SetDefaultCulture();

await builder.Build().RunAsync();
