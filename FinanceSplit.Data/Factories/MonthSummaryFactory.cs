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

        var transactions = await context
            .Transactions.Include(t => t.PaidBy)
            .Include(t => t.SharedWith)
            .Where(t => normalizedMonths.Contains(t.Month))
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync(cancellationToken: cancellationToken);

        var people = await context.People.ToListAsync(cancellationToken);

        foreach (var month in normalizedMonths)
        {
            var summary = summaries.GetValueOrDefault(month);
            if (summary is null)
            {
                summary = new MonthSummary() { Month = month };
                context.Add(summary);
            }

            summary.Update(people, transactions);
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
