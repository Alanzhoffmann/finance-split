using FinanceSplit.Contracts.Transactions.Commands;
using FinanceSplit.Contracts.Transactions.DTOs;
using FinanceSplit.Contracts.Transactions.Enums;
using FinanceSplit.Contracts.Transactions.Services;
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
            .Transactions.Include(t => t.Recurrence)
            .AsNoTrackingWithIdentityResolution()
            .OrderBy(t => t.Name)
            .Where(t => normalizedMonths.Contains(t.Month))
            .ToListAsync(cancellationToken: cancellationToken);

        List<TransactionDto> transactionList =
        [
            .. transactions.Where(t => t is not Recurrence).Select(t => t.ToDto()),
        ];

        var recurrences = await context
            .Recurrences.Where(r => !r.Ended)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync(cancellationToken: cancellationToken);

        foreach (var month in months)
        {
            foreach (var recurrence in recurrences)
            {
                var recurringTransactionForMonth = recurrence.GetTransactionForMonth(month);
                if (
                    recurringTransactionForMonth is not null
                    && (
                        recurringTransactionForMonth.Id == default
                        || !transactionList.Any(t => t.Id == recurringTransactionForMonth.Id)
                    )
                )
                {
                    transactionList.Add(recurringTransactionForMonth.ToDto());
                }
            }
        }

        transactionList.Sort((x, y) => x.Name.CompareTo(y.Name));
        return transactionList;
    }

    public async Task<TransactionDto?> CreateTransactionAsync(
        CreateTransactionCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var (paidByPerson, sharedWith) = await GetPeopleForTransaction(
            command.PaidByPersonId,
            command.SharedWithIds,
            cancellationToken
        );

        var newTransaction = command.RecurrenceSetup is { } recurrenceSetup
            ? new Recurrence
            {
                Name = command.Name,
                Month = command.Month,
                Value = command.Value,
                PaidBy = paidByPerson,
                SplitType = command.SplitType,
                SharedWith = sharedWith,
                NumberOfRecurrences = recurrenceSetup.NumberOfRecurrences,
                LastMonth = recurrenceSetup.LastMonth,
                Type = recurrenceSetup.Type,
            }
            : new Transaction
            {
                Name = command.Name,
                Month = command.Month,
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

        return await UpdateTransactionAsync(
            command.Name,
            command.Month,
            command.Value,
            command.PaidByPersonId,
            command.SplitType,
            command.SharedWithIds,
            transaction,
            cancellationToken
        );
    }

    public async Task<TransactionDto?> UpdateRecurringTransactionAsync(
        UpdateRecurringTransactionCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var recurrence = await context.Recurrences.FindAsync(
            [command.RecurrenceId],
            cancellationToken
        );
        if (recurrence is null)
        {
            return null;
        }

        Transaction? transaction;
        if (command.RecurrenceSetup.UpdateRecurrence is UpdateRecurrence.Future)
        {
            recurrence.EndRecurrence(command.Month);

            var (paidByPerson, sharedWith) = await GetPeopleForTransaction(
                command.PaidByPersonId,
                command.SharedWithIds,
                cancellationToken
            );

            transaction = new Recurrence
            {
                Name = command.Name,
                Month = command.Month,
                Value = command.Value,
                PaidBy = paidByPerson,
                SplitType = command.SplitType,
                SharedWith = sharedWith,
                NumberOfRecurrences =
                    command.RecurrenceSetup.NumberOfRecurrences - recurrence.Transactions.Count,
                LastMonth = command.RecurrenceSetup.LastMonth,
                Type = command.RecurrenceSetup.Type,
            };

            context.Transactions.Add(transaction);

            return transaction.ToDto();
        }
        else if (command.RecurrenceSetup.UpdateRecurrence is UpdateRecurrence.Transaction)
        {
            transaction = recurrence.GetTransactionForMonth(command.Month);
            if (transaction is null)
            {
                return null;
            }

            if (transaction.Id == default)
            {
                context.Add(transaction);
            }
        }

        return await UpdateTransactionAsync(
            command.Name,
            command.Month,
            command.Value,
            command.PaidByPersonId,
            command.SplitType,
            command.SharedWithIds,
            recurrence,
            cancellationToken
        );
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

    private async Task<(Person PaidByPerson, List<Person> SharedWith)> GetPeopleForTransaction(
        Guid paidByPersonId,
        IReadOnlyCollection<Guid> sharedWithIds,
        CancellationToken cancellationToken
    )
    {
        HashSet<Guid> personIds = [paidByPersonId, .. sharedWithIds];
        var people = await context
            .People.Where(p => personIds.Contains(p.Id))
            .ToListAsync(cancellationToken: cancellationToken);

        var paidByPerson = people.FirstOrDefault(p => p.Id == paidByPersonId);
        var sharedWith = people.Where(p => sharedWithIds.Contains(p.Id)).ToList();

        return paidByPerson is not null && sharedWith.Count == sharedWithIds.Count
            ? (paidByPerson, sharedWith)
            : throw new InvalidOperationException("Missing people in the database");
    }

    private async Task<TransactionDto?> UpdateTransactionAsync(
        string name,
        DateOnly month,
        decimal value,
        Guid paidByPersonId,
        SplitType splitType,
        IReadOnlyCollection<Guid> sharedWithIds,
        Transaction transaction,
        CancellationToken cancellationToken = default
    )
    {
        if (
            transaction.PaidBy.Id != paidByPersonId
            || !transaction.SharedWith.Select(p => p.Id).SequenceEqual(sharedWithIds)
        )
        {
            var (paidByPerson, sharedWith) = await GetPeopleForTransaction(
                paidByPersonId,
                sharedWithIds,
                cancellationToken
            );

            transaction.UpdatePaidBy(paidByPerson);
            transaction.UpdateSharedWith(sharedWith);
        }

        transaction.Update(name, month, value, splitType);

        await context.SaveChangesAsync(cancellationToken);

        return transaction.ToDto();
    }
}
