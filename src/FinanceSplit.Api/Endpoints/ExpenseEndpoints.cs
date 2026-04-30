using FinanceSplit.Application.Queries;

namespace FinanceSplit.Api.Endpoints;

public static class ExpenseEndpoints
{
    public static void MapExpenseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/expenses").WithTags("Expenses");

        group.MapGet(
            "/summary/{year:int}/{month:int}",
            async (int year, int month, ExpenseQueryService service, CancellationToken ct) =>
            {
                var monthDate = new DateOnly(year, month, 1);
                var summary = await service.GetMonthlySummaryAsync(monthDate, ct);
                return Results.Ok(summary);
            }
        );

        group.MapGet(
            "/settlements/{year:int}/{month:int}",
            async (int year, int month, ExpenseQueryService service, CancellationToken ct) =>
            {
                var monthDate = new DateOnly(year, month, 1);
                var settlements = await service.GetSettlementsAsync(monthDate, ct);
                return Results.Ok(settlements);
            }
        );
    }
}
