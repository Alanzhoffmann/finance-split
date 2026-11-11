using FinanceSplit.Data.BackgroundServices;
using FinanceSplit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FinanceSplit.Data.Interceptors;

internal partial class CalculatorInterceptor(CalculatorBackgroundService calculatorBackgroundService)
    : SaveChangesInterceptor
{
    private HashSet<CalculatorKey> _calculatorEvents = [];

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

        _calculatorEvents.Clear();

        var updatedIncomeMonths = context
            .ChangeTracker.Entries<IncomeRecord>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(i => new IncomeRecordChanged(i.Entity.Month));
        var updatedTransactions = context
            .ChangeTracker.Entries<Transaction>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(i => new TransactionChanged(i.Entity.Month));

        _calculatorEvents = [.. updatedIncomeMonths, .. updatedTransactions];

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var item in _calculatorEvents)
        {
            calculatorBackgroundService.Queue.TryEnqueue(item);
        }

        _calculatorEvents.Clear();

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}
