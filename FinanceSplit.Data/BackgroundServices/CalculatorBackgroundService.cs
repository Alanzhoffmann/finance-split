using FinanceSplit.Data.Internal;
using FinanceSplit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FinanceSplit.Data.BackgroundServices;

internal partial class CalculatorBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<CalculatorBackgroundService> logger
) : BackgroundService
{
    public CoalescingWorkQueue<CalculatorKey> Queue { get; } = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LogStarting();
        stoppingToken.Register(LogStopping);

        await foreach (CalculatorKey key in Queue.ReadAllAsync(stoppingToken))
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                switch (key)
                {
                    case IncomeRecordChanged incomeRecordChanged:
                        await CalculateIncomeRecordChanged(
                            scope.ServiceProvider,
                            incomeRecordChanged,
                            stoppingToken
                        );
                        break;
                    case TransactionChanged transactionChanged:
                        await CalculateTransactionChanged(
                            scope.ServiceProvider,
                            transactionChanged,
                            stoppingToken
                        );
                        break;
                }
            }
            finally
            {
                Queue.MarkProcessed(key);
            }
        }
    }

    private async Task CalculateIncomeRecordChanged(
        IServiceProvider serviceProvider,
        IncomeRecordChanged incomeRecordChanged,
        CancellationToken cancellationToken = default
    )
    {
        LogCalculatingSummaryForIncomeRecordChanged(incomeRecordChanged);

        var context = serviceProvider.GetRequiredService<AppDbContext>();
        var timeProvider = serviceProvider.GetRequiredService<TimeProvider>();

        // Get all summaries after income record changed, all can be affected
        var summaries = await context
            .Summaries.Where(s => s.Month >= incomeRecordChanged.Month)
            .ToDictionaryAsync(s => s.Month, cancellationToken: cancellationToken);

        var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().Date);
        HashSet<DateOnly> monthsToCalculate = [incomeRecordChanged.Month];
        for (DateOnly i = incomeRecordChanged.Month; i <= today; i = i.AddMonths(1))
        {
            monthsToCalculate.Add(i);
        }

        var transactions = await context
            .Transactions.Include(t => t.PaidBy)
            .Include(t => t.SharedWith)
            .Where(t => monthsToCalculate.Contains(t.Month))
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync(cancellationToken: cancellationToken);

        var people = await context.People.ToListAsync(cancellationToken);

        foreach (var month in monthsToCalculate)
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

    private async Task CalculateTransactionChanged(
        IServiceProvider serviceProvider,
        TransactionChanged transactionChanged,
        CancellationToken cancellationToken = default
    )
    {
        LogCalculatingSummaryForTransactionChanged(transactionChanged);
        var context = serviceProvider.GetRequiredService<AppDbContext>();

        var summary = await context.Summaries.FirstOrDefaultAsync(
            s => s.Month == transactionChanged.Month,
            cancellationToken: cancellationToken
        );

        var transactions = await context
            .Transactions.Include(t => t.PaidBy)
            .Include(t => t.SharedWith)
            .Where(t => t.Month == transactionChanged.Month)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync(cancellationToken: cancellationToken);

        var people = await context.People.ToListAsync(cancellationToken);

        if (summary is null)
        {
            summary = new MonthSummary() { Month = transactionChanged.Month };
            context.Add(summary);
        }

        summary.Update(people, transactions);

        await context.SaveChangesAsync(cancellationToken);
    }

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Starting calculator background service"
    )]
    partial void LogStarting();

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Stopping calculator background service"
    )]
    partial void LogStopping();

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Calculating summaries for income record changed {IncomeRecordChanged}"
    )]
    partial void LogCalculatingSummaryForIncomeRecordChanged(
        IncomeRecordChanged incomeRecordChanged
    );

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Calculating summary for transaction changed {TransactionChanged}"
    )]
    partial void LogCalculatingSummaryForTransactionChanged(TransactionChanged transactionChanged);
}
