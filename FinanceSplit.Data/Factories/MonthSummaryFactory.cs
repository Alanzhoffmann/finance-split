using FinanceSplit.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceSplit.Data.Factories;

internal class MonthSummaryFactory(AppDbContext context)
{
    public async Task UpdateMonths(
        IEnumerable<DateOnly> monthsToUpdate,
        CancellationToken cancellationToken = default
    )
    {
        var normalizedMonths = monthsToUpdate
            .Select(m => m.Day == 1 ? m : new DateOnly(m.Year, m.Month, 1))
            .ToList();
        var summaries = await context
            .Summaries.Where(s => normalizedMonths.Contains(s.Month))
            .ToDictionaryAsync(s => s.Month, cancellationToken: cancellationToken);

        var peopleWithTransactions = await GetPeopleWithTransactions(
            normalizedMonths,
            cancellationToken
        );
        foreach (var month in normalizedMonths)
        {
            var summary = summaries.GetValueOrDefault(month);
            if (summary is null)
            {
                summary = new MonthSummary() { Month = month };
                context.Add(summary);
            }

            summary.Update(peopleWithTransactions);
        }
    }

    private async Task<List<Person>> GetPeopleWithTransactions(
        IEnumerable<DateOnly> months,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .People.Include(p => p.Transactions.Where(t => months.Contains(t.Month)))
            .Include(p => p.SharedTransactions.Where(t => months.Contains(t.Month)))
            .ToListAsync(cancellationToken: cancellationToken);
}
