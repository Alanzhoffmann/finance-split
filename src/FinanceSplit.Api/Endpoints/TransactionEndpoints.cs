using FinanceSplit.Application.Services;
using FinanceSplit.Contracts.Enums;
using FinanceSplit.Contracts.Requests;
using FinanceSplit.Domain.ValueObjects;

namespace FinanceSplit.Api.Endpoints;

public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/transactions").WithTags("Transactions");

        group.MapGet(
            "/{id:guid}",
            async (Guid id, TransactionService service, CancellationToken ct) =>
            {
                var transaction = await service.GetTransactionAsync(id, ct);
                return transaction is null ? Results.NotFound() : Results.Ok(transaction);
            }
        );

        group.MapPost(
            "/",
            async (CreateTransactionRequest request, TransactionService service, CancellationToken ct) =>
            {
                Recurrence? recurrence = null;
                if (request.Recurrence is not null)
                {
                    recurrence = request.Recurrence.TerminationType switch
                    {
                        RecurrenceTerminationType.Open => Recurrence.Forever(request.Recurrence.StartMonth),
                        RecurrenceTerminationType.ByCount when request.Recurrence.OccurrenceCount.HasValue => Recurrence.ForMonths(
                            request.Recurrence.StartMonth,
                            request.Recurrence.OccurrenceCount.Value
                        ),
                        RecurrenceTerminationType.ByDate when request.Recurrence.EndMonth.HasValue => Recurrence.UntilMonth(
                            request.Recurrence.StartMonth,
                            request.Recurrence.EndMonth.Value
                        ),
                        _ => Recurrence.Forever(request.Recurrence.StartMonth),
                    };
                }

                var transaction = await service.CreateTransactionAsync(
                    request.Title,
                    request.Amount,
                    request.PaidById,
                    request.SplitType,
                    request.ParticipantIds,
                    request.Date,
                    recurrence,
                    ct
                );

                return transaction is null
                    ? Results.BadRequest("Invalid person references.")
                    : Results.Created($"/api/transactions/{transaction.Id}", transaction);
            }
        );
    }
}
