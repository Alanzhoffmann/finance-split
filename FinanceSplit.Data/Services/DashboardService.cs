using FinanceSplit.Contracts.DTOs;
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
        return await context
            .Summaries.AsNoTracking()
            .Where(s => s.Month == monthToCheck)
            .Select(s => new DashboardResponse(
                s.People.Select(p => new DashboardPersonResponse(
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
    }
}
