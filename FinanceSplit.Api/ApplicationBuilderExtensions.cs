using FinanceSplit.Api.Endpoints;
using FinanceSplit.Api.Middlewares;
using FinanceSplit.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;

namespace FinanceSplit.Api;

public static class ApplicationBuilderExtensions
{
    public static void AddApiServices(this IHostApplicationBuilder builder)
    {
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
        endpoints.MapDashboardEndpoints();
    }
}
