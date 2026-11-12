using FinanceSplit.Data.Internal;
using FinanceSplit.Data.Services;
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
                    case IncomeRecordDeleted incomeRecordDeleted:
                        await CalculateIncomeRecordDeleted(
                            scope.ServiceProvider,
                            incomeRecordDeleted,
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

    private static async Task RecalculateSummaryFromDates(
        AppDbContext context,
        TransactionService transactionService,
        IReadOnlyCollection<DateOnly> months,
        IEnumerable<Person>? people = null,
        CancellationToken cancellationToken = default
    )
    {
        var summaries = await context
            .Summaries.Where(s => months.Contains(s.Month))
            .ToDictionaryAsync(s => s.Month, cancellationToken: cancellationToken);

        var transactions = await transactionService.GetTransactionsAsync(months, cancellationToken);

        people ??= await context.People.ToListAsync(cancellationToken);

        foreach (var month in months)
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

    private static HashSet<DateOnly> GetAllMonthsBetween(DateOnly start, DateOnly end)
    {
        HashSet<DateOnly> monthsToCalculate = [start];
        for (DateOnly i = start; i <= end; i = i.AddMonths(1))
        {
            monthsToCalculate.Add(i);
        }

        return monthsToCalculate;
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
        var transactionService = serviceProvider.GetRequiredService<TransactionService>();

        // Get all months after income record changed, all can be affected
        var summaries = await context
            .Summaries.Where(s => s.Month >= incomeRecordChanged.Month)
            .ToDictionaryAsync(s => s.Month, cancellationToken: cancellationToken);

        var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().Date);
        var monthsToCalculate = GetAllMonthsBetween(incomeRecordChanged.Month, today);

        await RecalculateSummaryFromDates(
            context,
            transactionService,
            monthsToCalculate,
            cancellationToken: cancellationToken
        );
    }

    private async Task CalculateIncomeRecordDeleted(
        IServiceProvider serviceProvider,
        IncomeRecordDeleted incomeRecordDeleted,
        CancellationToken cancellationToken = default
    )
    {
        LogCalculatingSummaryForIncomeRecordDeleted(incomeRecordDeleted);

        var context = serviceProvider.GetRequiredService<AppDbContext>();
        var timeProvider = serviceProvider.GetRequiredService<TimeProvider>();
        var transactionService = serviceProvider.GetRequiredService<TransactionService>();

        var people = await context.People.ToListAsync(cancellationToken);

        // Get all months since previous income, all can be affected
        DateOnly previousIncomeMonth = people.Min(p =>
            p.IncomeRecords.OrderByDescending(i => i.Month)
                .FirstOrDefault(i => i.Month < incomeRecordDeleted.Month)
                ?.Month
            ?? DateOnly.MaxValue
        );
        if (previousIncomeMonth == DateOnly.MaxValue)
        {
            previousIncomeMonth = incomeRecordDeleted.Month;
        }

        var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().Date);
        var monthsToCalculate = GetAllMonthsBetween(previousIncomeMonth, today);

        await RecalculateSummaryFromDates(
            context,
            transactionService,
            monthsToCalculate,
            people,
            cancellationToken
        );
    }

    private async Task CalculateTransactionChanged(
        IServiceProvider serviceProvider,
        TransactionChanged transactionChanged,
        CancellationToken cancellationToken = default
    )
    {
        LogCalculatingSummaryForTransactionChanged(transactionChanged);

        var context = serviceProvider.GetRequiredService<AppDbContext>();
        var transactionService = serviceProvider.GetRequiredService<TransactionService>();

        await RecalculateSummaryFromDates(
            context,
            transactionService,
            [transactionChanged.Month],
            cancellationToken: cancellationToken
        );
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
}
