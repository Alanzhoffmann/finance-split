using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;
using Microsoft.EntityFrameworkCore;

namespace FinanceSplit.Data.Services;

internal class TransactionService(AppDbContext context) : ITransactionService
{
    public async Task<IReadOnlyList<TransactionResponse>?> GetTransactionsAsync(
        DateTime? month = null,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .Transactions.Select(t => new TransactionResponse(
                t.Id,
                t.Name,
                t.Value,
                t.Date,
                t.SplitType,
                new TransactionPersonResponse(t.PaidBy.Id, t.PaidBy.Name),
                t.IsRecurring,
                t.SharedWith.Select(tp => new TransactionPersonResponse(tp.Id, tp.Name)).ToList()
            ))
            .ToListAsync(cancellationToken: cancellationToken);
}
