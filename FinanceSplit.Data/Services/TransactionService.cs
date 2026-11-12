using FinanceSplit.Contracts.Commands;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;
using FinanceSplit.Domain.Entities;
using FinanceSplit.Domain.Mappers;
using Microsoft.EntityFrameworkCore;

namespace FinanceSplit.Data.Services;

internal class TransactionService(AppDbContext context, TimeProvider timeProvider)
    : ITransactionService
{
    public async Task<IReadOnlyList<TransactionDto>?> GetTransactionsAsync(
        DateOnly? month = null,
        CancellationToken cancellationToken = default
    )
    {
        month ??= DateOnly.FromDateTime(timeProvider.GetLocalNow().Date);

        var monthInit = new DateOnly(month.Value.Year, month.Value.Month, 1);
        var monthEnd = monthInit.AddMonths(1);

        var transactions = await context
            .Transactions.AsNoTrackingWithIdentityResolution()
            .OrderBy(t => t.Name)
            .Where(t => monthInit <= t.Month && t.Month < monthEnd)
            .ToListAsync(cancellationToken: cancellationToken);

        return [.. transactions.Select(t => t.ToDto())];
    }

    public async Task<TransactionDto?> CreateTransactionAsync(
        CreateTransactionCommand command,
        CancellationToken cancellationToken = default
    )
    {
        HashSet<Guid> personIds = [command.PaidByPersonId, .. command.SharedWithIds];
        var people = await context
            .People.Where(p => personIds.Contains(p.Id))
            .ToListAsync(cancellationToken: cancellationToken);

        var paidByPerson = people.FirstOrDefault(p => p.Id == command.PaidByPersonId);
        if (paidByPerson is null)
        {
            return null;
        }

        var sharedWith = people.Where(p => command.SharedWithIds.Contains(p.Id)).ToList();
        if (sharedWith.Count < command.SharedWithIds.Count)
        {
            // Not every person shared with is accounted for
            return null;
        }

        var newTransaction = new Transaction
        {
            Name = command.Name,
            Month = command.Date,
            Value = command.Value,
            PaidBy = paidByPerson,
            SplitType = command.SplitType,
            SharedWith = sharedWith,
        };
        context.Transactions.Add(newTransaction);

        await context.SaveChangesAsync(cancellationToken);

        return newTransaction.ToDto();
    }

    public async Task<TransactionDto?> UpdateTransactionAsync(
        UpdateTransactionCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var transaction = await context.Transactions.FindAsync([command.Id], cancellationToken);
        if (transaction is null)
        {
            return null;
        }

        if (
            transaction.PaidBy.Id != command.PaidByPersonId
            || !transaction.SharedWith.Select(p => p.Id).SequenceEqual(command.SharedWithIds)
        )
        {
            HashSet<Guid> personIds = [command.PaidByPersonId, .. command.SharedWithIds];
            var people = await context
                .People.Where(p => personIds.Contains(p.Id))
                .ToListAsync(cancellationToken: cancellationToken);

            var paidByPerson = people.FirstOrDefault(p => p.Id == command.PaidByPersonId);
            if (paidByPerson is null)
            {
                return null;
            }

            var sharedWith = people.Where(p => command.SharedWithIds.Contains(p.Id)).ToList();
            if (sharedWith.Count < command.SharedWithIds.Count)
            {
                // Not every person shared with is accounted for
                return null;
            }

            transaction.UpdatePaidBy(paidByPerson);
            transaction.UpdateSharedWith(sharedWith);
        }

        transaction.Update(command.Name, command.Date, command.Value, command.SplitType);

        await context.SaveChangesAsync(cancellationToken);

        return transaction.ToDto();
    }

    public async Task DeleteTransactionAsync(
        DeleteTransactionCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var transactionToDelete = await context.Transactions.FindAsync(
            [command.Id],
            cancellationToken
        );
        if (transactionToDelete is null)
        {
            // TODO figure out 404s here
            return;
        }

        context.Remove(transactionToDelete);

        await context.SaveChangesAsync(cancellationToken);
    }
}
