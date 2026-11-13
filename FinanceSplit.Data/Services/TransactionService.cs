using FinanceSplit.Contracts.Services;
using FinanceSplit.Contracts.Transactions.Commands;
using FinanceSplit.Contracts.Transactions.Dtos;
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
        return await GetTransactionsAsync([month.Value], cancellationToken);
    }

    public async Task<IReadOnlyList<TransactionDto>> GetTransactionsAsync(
        IEnumerable<DateOnly> months,
        CancellationToken cancellationToken = default
    )
    {
        var normalizedMonths = months.Select(m =>
            m.Day == 1 ? m : new DateOnly(m.Year, m.Month, 1)
        );

        var transactions = await context
            .Transactions.AsNoTrackingWithIdentityResolution()
            .OrderBy(t => t.Name)
            .Where(t => normalizedMonths.Contains(t.Month))
            .ToListAsync(cancellationToken: cancellationToken);

        List<TransactionDto> transactionList =
        [
            .. transactions.Where(t => t is not Recurrence).Select(t => t.ToDto()),
        ];

        var recurrences = await context
            .Recurrences.Where(r => !r.Ended)
            .ToListAsync(cancellationToken: cancellationToken);
        foreach (var month in months)
        {
            foreach (var recurrence in recurrences)
            {
                var recurringTransactionForMonth = recurrence.GetTransactionForMonth(month);
                if (
                    recurringTransactionForMonth is null
                    || transactionList.Any(t => t.Id == recurringTransactionForMonth.Id)
                )
                {
                    continue;
                }

                transactionList.Add(recurringTransactionForMonth.ToDto());
            }
        }

        return transactionList;
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
