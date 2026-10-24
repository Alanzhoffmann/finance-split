using FinanceSplit.Api.Endpoints;
using FinanceSplit.Api.Middlewares;
using FinanceSplit.Contracts.Services;
using FinanceSplit.Data;
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
        builder.Services.AddScoped<IWeatherService, WeatherService>();

        builder.Services.AddDatabase();
    }

    public static void UseApiServices(this IApplicationBuilder app)
    {
        app.UseMiddleware<MigrationMiddleware>();
    }

    public static void MapApiEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPersonEndpoints();
        endpoints.MapTransactionEndpoints();

        endpoints.MapGet(
            "api/weather",
            async (
                [FromServices] IWeatherService weatherService,
                CancellationToken cancellationToken = default
            ) => await weatherService.GetWeatherForecastsAsync(cancellationToken)
        );
    }
}
