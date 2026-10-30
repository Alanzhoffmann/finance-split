using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Enums;
using FinanceSplit.Contracts.Services;
using Microsoft.EntityFrameworkCore;

namespace FinanceSplit.Data.Services;

internal class DashboardService(AppDbContext context, TimeProvider timeProvider) : IDashboardService
{
    public async Task<DashboardResponse?> GetDashboardAsync(
        DateOnly? month = null,
        CancellationToken cancellationToken = default
    )
    {
        var monthToCheck = month ?? DateOnly.FromDateTime(timeProvider.GetLocalNow().Date);
        var people = await context
            .People.Include(p =>
                p.Transactions.Where(t =>
                    t.Date.Month == monthToCheck.Month && t.Date.Year == monthToCheck.Year
                )
            )
            .Include(p =>
                p.SharedTransactions.Where(t =>
                    t.Date.Month == monthToCheck.Month && t.Date.Year == monthToCheck.Year
                )
            )
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync(cancellationToken: cancellationToken);

        Dictionary<Guid, decimal> totalSharedPerPersonId = people
            .SelectMany(p => p.SharedTransactions)
            .DistinctBy(t => t.Id)
            .Where(t => t.SplitType is not SplitType.None)
            .SelectMany(t => t.CalculateShares())
            .GroupBy(g => g.Key.Id)
            .ToDictionary(g => g.Key, g => g.Sum(g1 => g1.Value));

        return new DashboardResponse([
            .. people.Select(p => new DashboardPersonResponse(
                p.Name,
                p.GetIncomeForMonth(monthToCheck),
                totalSharedPerPersonId.GetValueOrDefault(p.Id),
                p.Transactions.Where(t =>
                        t.SplitType is SplitType.None
                        && (t.SharedWith.Count == 0 || t.SharedWith[0].Id == t.PaidBy.Id)
                    )
                    .Sum(t => t.Value),
                p.Transactions.Sum(t => t.Value)
            )),
        ]);
    }
}
