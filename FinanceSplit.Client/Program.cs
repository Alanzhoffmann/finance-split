using FinanceSplit.Client.Clients;
using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
});

builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<IPersonService, PersonService>();

await builder.Build().RunAsync();
