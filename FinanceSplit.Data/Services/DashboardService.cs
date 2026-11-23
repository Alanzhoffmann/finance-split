using FinanceSplit.Contracts.Dashboard.DTOs;
using FinanceSplit.Contracts.Dashboard.Services;
using FinanceSplit.Contracts.Transactions.Services;
using FinanceSplit.Domain.Entities;
using FinanceSplit.Domain.Mappers;
using Microsoft.EntityFrameworkCore;

namespace FinanceSplit.Data.Services;

internal class DashboardService(
    AppDbContext context,
    ITransactionService transactionService,
    TimeProvider timeProvider
) : IDashboardService
{
    public async Task<DashboardDto?> GetDashboardAsync(
        DateOnly? month = null,
        CancellationToken cancellationToken = default
    )
    {
        var monthToCheck = month ?? DateOnly.FromDateTime(timeProvider.GetLocalNow().Date);

        return new DashboardDto(
            await GetMonthSummaryAsync(monthToCheck, cancellationToken),
            await GetGoalsAsync(cancellationToken)
        );
    }

    private async Task<IReadOnlyList<GoalDto>> GetGoalsAsync(CancellationToken cancellationToken)
    {
        var goals = await context
            .Goals.AsNoTrackingWithIdentityResolution()
            .ToListAsync(cancellationToken: cancellationToken);

        return [.. goals.Select(g => g.ToDto())];
    }

    private async Task<MonthSummaryDto> GetMonthSummaryAsync(
        DateOnly monthToCheck,
        CancellationToken cancellationToken
    )
    {
        var monthSummary = await context
            .Summaries.AsNoTracking()
            .Where(s => s.Month == monthToCheck)
            .Select(s => new MonthSummaryDto(
                s.People.Select(p => new PersonSummaryDto(
                        p.Person.Name,
                        p.Income,
                        p.SharedExpenses,
                        p.PersonalExpenses,
                        p.TotalPaid
                    ))
                    .ToList(),
                s.Settlements.Select(st => new ExpenseSettlementDto(
                        new(st.From.Id, st.From.Name),
                        new(st.To.Id, st.To.Name),
                        st.Amount
                    ))
                    .ToList()
            ))
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (monthSummary is null)
        {
            // calculate if missing
            // TODO move this to a dedicated factory
            var people = await context.People.AsNoTracking().ToListAsync(cancellationToken);
            var transactions = await transactionService.GetTransactionsAsync(
                [monthToCheck],
                cancellationToken
            );

            var projectedMonthSummary = new MonthSummary() { Month = monthToCheck };
            projectedMonthSummary.Update(people, transactions);

            monthSummary = new MonthSummaryDto(
                [
                    .. projectedMonthSummary.People.Select(p => new PersonSummaryDto(
                        p.Person.Name,
                        p.Income,
                        p.SharedExpenses,
                        p.PersonalExpenses,
                        p.TotalPaid
                    )),
                ],
                [
                    .. projectedMonthSummary.Settlements.Select(st => new ExpenseSettlementDto(
                        new(st.From.Id, st.From.Name),
                        new(st.To.Id, st.To.Name),
                        st.Amount
                    )),
                ]
            );
        }

        return monthSummary;
    }
}
