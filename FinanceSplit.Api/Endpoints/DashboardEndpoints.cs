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
                [FromQuery] string? month,
                IDashboardService service,
                CancellationToken cancellationToken = default
            ) =>
            {
                DateOnly? targetMonth =
                    !string.IsNullOrWhiteSpace(month)
                    && DateOnly.TryParseExact(month, "yyyy-MM", out var selectedMonth)
                        ? new DateOnly(selectedMonth.Year, selectedMonth.Month, 1)
                        : null;

                var dashboard = await service.GetDashboardAsync(targetMonth, cancellationToken);
                return TypedResults.Ok(dashboard);
            }
        );

        return group;
    }
}
