using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FinanceSplit.Api;

public static class ApplicationBuilderExtensions
{
    public static void AddApiServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IWeatherSevice, WeatherService>();
    }

    public static void UseApiServices(this IApplicationBuilder app)
    {

    }

    public static void MapApiEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("api/weather", async ([FromServices] IWeatherSevice weatherSevice, CancellationToken cancellationToken = default) => await weatherSevice.GetWeatherForecastsAsync(cancellationToken));
    }
}
