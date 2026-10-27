using FinanceSplit.Contracts.Commands;
using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FinanceSplit.Api.Endpoints;

internal static class DashboardEndpoints
{
    public static RouteGroupBuilder MapDashboardEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("api/dashboard").WithTags("Dashboard");

        group.MapGet(
            "",
            async (
                [FromQuery] DateOnly? month,
                IDashboardService service,
                CancellationToken cancellationToken = default
            ) =>
            {
                var dashboard = await service.GetDashboardAsync(month, cancellationToken);
                return TypedResults.Ok(dashboard);
            }
        );

        return group;
    }
}
