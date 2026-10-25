using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;
using FinanceSplit.Domain.Entities;
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

    public async Task<TransactionResponse?> CreateTransactionAsync(
        CreateTransactionCommand command,
        CancellationToken cancellationToken = default
    )
    {
        Guid[] personIds = [command.PaidByPersonId, .. command.SharedWithIds];
        var people = await context
            .People.Where(p => personIds.Contains(p.Id))
            .ToListAsync(cancellationToken: cancellationToken);

        var paidByPerson = people.FirstOrDefault(p => p.Id == command.PaidByPersonId);
        if (paidByPerson is null)
        {
            return null;
        }

        var sharedWith = people.Where(p => personIds.Contains(p.Id)).ToList();
        if (sharedWith.Count < command.SharedWithIds.Length)
        {
            // Not every person shared with is accounted for
            return null;
        }

        var newTransaction = new Transaction
        {
            Name = command.Name,
            Date = command.Date,
            Value = command.Value,
            PaidBy = paidByPerson,
            IsRecurring = command.IsRecurring,
            SplitType = command.SplitType,
            SharedWith = sharedWith,
        };
        context.Transactions.Add(newTransaction);

        await context.SaveChangesAsync(cancellationToken);

        return new TransactionResponse(
            newTransaction.Id,
            newTransaction.Name,
            newTransaction.Value,
            newTransaction.Date,
            newTransaction.SplitType,
            new(newTransaction.PaidBy.Id, newTransaction.PaidBy.Name),
            newTransaction.IsRecurring,
            [.. newTransaction.SharedWith.Select(p => new TransactionPersonResponse(p.Id, p.Name))]
        );
    }
}
