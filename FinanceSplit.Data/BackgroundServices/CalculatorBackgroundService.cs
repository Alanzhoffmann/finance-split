using FinanceSplit.Contracts.Transactions.Services;
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
                        await CalculateIncomeRecordChangedAsync(
                            scope.ServiceProvider,
                            incomeRecordChanged,
                            stoppingToken
                        );
                        break;
                    case IncomeRecordDeleted incomeRecordDeleted:
                        await CalculateIncomeRecordDeletedAsync(
                            scope.ServiceProvider,
                            incomeRecordDeleted,
                            stoppingToken
                        );
                        break;
                    case TransactionChanged transactionChanged:
                        CalculateTransactionChanged(transactionChanged);
                        break;
                    case CalculateSummary calculateSummary:
                        await RunCalculateSummary(
                            scope.ServiceProvider,
                            calculateSummary,
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

    private static HashSet<DateOnly> GetAllMonthsBetween(DateOnly start, DateOnly end)
    {
        HashSet<DateOnly> months = [start];
        for (DateOnly i = start; i <= end; i = i.AddMonths(1))
        {
            months.Add(i);
        }

        return months;
    }

    private async Task CalculateIncomeRecordChangedAsync(
        IServiceProvider serviceProvider,
        IncomeRecordChanged incomeRecordChanged,
        CancellationToken cancellationToken = default
    )
    {
        LogCalculatingSummaryForIncomeRecordChanged(incomeRecordChanged);

        var context = serviceProvider.GetRequiredService<AppDbContext>();
        var timeProvider = serviceProvider.GetRequiredService<TimeProvider>();

        var lastSummaryMonth = await context
            .Summaries.Select(s => s.Month)
            .DefaultIfEmpty()
            .MaxAsync(cancellationToken: cancellationToken);

        // Get all months after income record changed, all can be affected
        foreach (var month in GetAllMonthsBetween(incomeRecordChanged.Month, lastSummaryMonth))
        {
            Queue.TryEnqueue(new CalculateSummary(month));
        }
    }

    private async Task CalculateIncomeRecordDeletedAsync(
        IServiceProvider serviceProvider,
        IncomeRecordDeleted incomeRecordDeleted,
        CancellationToken cancellationToken = default
    )
    {
        LogCalculatingSummaryForIncomeRecordDeleted(incomeRecordDeleted);

        var context = serviceProvider.GetRequiredService<AppDbContext>();
        var timeProvider = serviceProvider.GetRequiredService<TimeProvider>();

        // Get all months since previous income, all can be affected
        DateOnly previousIncomeMonth = await context
            .People.Select(p =>
                p.IncomeRecords.OrderByDescending(i => i.Month)
                    .FirstOrDefault(i => i.Month < incomeRecordDeleted.Month)
            )
            .MinAsync(
                i => i == null ? DateOnly.MaxValue : i.Month,
                cancellationToken: cancellationToken
            );

        if (previousIncomeMonth == DateOnly.MaxValue)
        {
            previousIncomeMonth = incomeRecordDeleted.Month;
        }

        var lastSummaryMonth = await context
            .Summaries.Select(s => s.Month)
            .DefaultIfEmpty()
            .MaxAsync(cancellationToken: cancellationToken);

        foreach (var month in GetAllMonthsBetween(previousIncomeMonth, lastSummaryMonth))
        {
            Queue.TryEnqueue(new CalculateSummary(month));
        }
    }

    private void CalculateTransactionChanged(TransactionChanged transactionChanged)
    {
        LogCalculatingSummaryForTransactionChanged(transactionChanged);

        Queue.TryEnqueue(new CalculateSummary(transactionChanged.Month));
    }

    private async Task RunCalculateSummary(
        IServiceProvider serviceProvider,
        CalculateSummary calculateSummary,
        CancellationToken cancellationToken = default
    )
    {
        LogCalculatingSummary(calculateSummary.Month);

        var context = serviceProvider.GetRequiredService<AppDbContext>();
        var transactionService = serviceProvider.GetRequiredService<ITransactionService>();

        var summary = await context
            .Summaries.AsSingleQuery()
            .FirstOrDefaultAsync(
                s => calculateSummary.Month == s.Month,
                cancellationToken: cancellationToken
            );

        var transactions = await transactionService.GetTransactionsAsync(
            calculateSummary.Month,
            cancellationToken
        );

        var people = await context.People.ToListAsync(cancellationToken);

        if (summary is null)
        {
            summary = new MonthSummary() { Month = calculateSummary.Month };
            context.Add(summary);
        }

        summary.Update(people, transactions ?? []);

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
        Message = "Calculating summaries for income record deleted {IncomeRecordDeleted}"
    )]
    partial void LogCalculatingSummaryForIncomeRecordDeleted(
        IncomeRecordDeleted incomeRecordDeleted
    );

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Calculating summary for transaction changed {TransactionChanged}"
    )]
    partial void LogCalculatingSummaryForTransactionChanged(TransactionChanged transactionChanged);

    [LoggerMessage(Level = LogLevel.Information, Message = "Calculating summary for month {Month}")]
    partial void LogCalculatingSummary(DateOnly month);
}
