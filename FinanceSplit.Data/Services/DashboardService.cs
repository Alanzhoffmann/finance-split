using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Enums;
using FinanceSplit.Contracts.Services;
using FinanceSplit.Domain.Entities;
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

        var people = await GetPeopleWithTransactions(monthToCheck, cancellationToken);
        var totalSharedPerPersonId = GetTotalSharedPerPerson(people.Values);

        var settlements = new MonthlyExpenseSummary(
            people.Values.SelectMany(p => p.Transactions).DistinctBy(t => t.Id)
        ).CalculateSettlements();

        return new DashboardResponse(
            [
                .. people.Values.Select(p => new DashboardPersonResponse(
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
            ],
            [
                .. settlements.Select(s => new ExpenseSettlementResponse(
                    people.TryGetValue(s.FromPersonId, out var from)
                        ? from.Name
                        : throw new NullReferenceException("from missing"),
                    people.TryGetValue(s.ToPersonId, out var to)
                        ? to.Name
                        : throw new NullReferenceException("to missing"),
                    s.Amount
                )),
            ]
        );
    }

    private async Task<Dictionary<Guid, Person>> GetPeopleWithTransactions(
        DateOnly month,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .People.Include(p =>
                p.Transactions.Where(t => t.Date.Month == month.Month && t.Date.Year == month.Year)
            )
            .Include(p =>
                p.SharedTransactions.Where(t =>
                    t.Date.Month == month.Month && t.Date.Year == month.Year
                )
            )
            .AsNoTrackingWithIdentityResolution()
            .ToDictionaryAsync(p => p.Id, p => p, cancellationToken: cancellationToken);

    private static Dictionary<Guid, decimal> GetTotalSharedPerPerson(IEnumerable<Person> people) =>
        people
            .SelectMany(p => p.SharedTransactions)
            .DistinctBy(t => t.Id)
            .Where(t => t.SplitType is not SplitType.None)
            .SelectMany(t => t.CalculateShares())
            .GroupBy(g => g.Key.Id)
            .ToDictionary(g => g.Key, g => g.Sum(g1 => g1.Value));
}
