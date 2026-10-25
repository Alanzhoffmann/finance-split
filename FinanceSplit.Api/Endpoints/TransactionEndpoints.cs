using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FinanceSplit.Api.Endpoints;

internal static class TransactionEndpoints
{
    public static RouteGroupBuilder MapTransactionEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("api/transaction").WithTags("Transactions");

        group.MapGet(
            "",
            async (
                DateTime? month,
                ITransactionService transactionService,
                CancellationToken cancellationToken = default
            ) =>
            {
                var transactions = await transactionService.GetTransactionsAsync(
                    month,
                    cancellationToken
                );
                return TypedResults.Ok(transactions);
            }
        );

        group.MapPost(
            "",
            async (
                CreateTransactionCommand command,
                ITransactionService transactionService,
                CancellationToken cancellationToken = default
            ) =>
            {
                var transaction = await transactionService.CreateTransactionAsync(
                    command,
                    cancellationToken
                );

                return transaction is null ? Results.NotFound() : TypedResults.Ok(transaction);
            }
        );

        return group;
    }
}
