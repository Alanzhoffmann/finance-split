using FinanceSplit.Data.Factories;
using FinanceSplit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceSplit.Data.Interceptors;

internal partial class MonthSummaryCalculatorInterceptor(IServiceScopeFactory scopeFactory)
    : SaveChangesInterceptor
{
    private List<DateOnly> _monthsToUpdate = [];

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

        _monthsToUpdate.Clear();

        var updatedIncomeMonths = context
            .ChangeTracker.Entries<IncomeRecord>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(i => i.Entity.Month);
        var updatedTransactions = context
            .ChangeTracker.Entries<Transaction>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(i => i.Entity.Month);

        _monthsToUpdate = [.. updatedIncomeMonths, .. updatedTransactions];

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default
    )
    {
        if (_monthsToUpdate.Count > 0)
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var factory = scope.ServiceProvider.GetRequiredService<MonthSummaryFactory>();

            var monthsToUpdate = _monthsToUpdate.ToList();
            _monthsToUpdate.Clear();

            await factory.UpdateMonths(monthsToUpdate, cancellationToken);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}
