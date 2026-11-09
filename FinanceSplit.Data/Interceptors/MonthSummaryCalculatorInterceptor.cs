using FinanceSplit.Data.Factories;
using FinanceSplit.Domain.Entities;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FinanceSplit.Data.Interceptors;

internal partial class MonthSummaryCalculatorInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        if (eventData.Context is not AppDbContext context)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var updatedIncomeMonths = context
            .ChangeTracker.Entries<IncomeRecord>()
            .Select(i => i.Entity.Month);
        var updatedTransactions = context
            .ChangeTracker.Entries<Transaction>()
            .Select(i => i.Entity.Month);

        var factory = new MonthSummaryFactory(context);
        await factory.UpdateMonths(
            [.. updatedIncomeMonths, .. updatedTransactions],
            cancellationToken
        );

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
