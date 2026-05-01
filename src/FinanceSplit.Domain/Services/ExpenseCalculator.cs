using FinanceSplit.Domain.Entities;
using FinanceSplit.Domain.ValueObjects;

namespace FinanceSplit.Domain.Services;

public static class ExpenseCalculator
{
    /// <summary>
    /// Builds a monthly summary from one-off transactions only. The month is inferred
    /// from the first transaction's date. Use <see cref="BuildMonthlySummary(DateOnly, IEnumerable{Transaction})"/>
    /// when the transaction set includes recurring transactions.
    /// </summary>
    public static MonthlyExpenseSummary BuildMonthlySummary(IEnumerable<Transaction> monthTransactions)
    {
        ArgumentNullException.ThrowIfNull(monthTransactions);

        var transactions = monthTransactions.ToArray();
        if (transactions.Length == 0)
        {
            throw new ArgumentException("At least one transaction is required.", nameof(monthTransactions));
        }

        var month = DateOnly.FromDateTime(transactions[0].Date);
        return new MonthlyExpenseSummary(month, transactions);
    }

    /// <summary>
    /// Builds a monthly summary for the given month from a mix of one-off and recurring
    /// transactions. Recurring transactions are validated against their recurrence schedule;
    /// one-off transactions must fall in the target month.
    /// </summary>
    public static MonthlyExpenseSummary BuildMonthlySummary(DateOnly month, IEnumerable<Transaction> transactions)
    {
        ArgumentNullException.ThrowIfNull(transactions);

        return new MonthlyExpenseSummary(month, transactions);
    }

    public static IReadOnlyCollection<ExpenseSettlement> CalculateSettlements(IEnumerable<Transaction> monthTransactions)
    {
        var summary = BuildMonthlySummary(monthTransactions);
        return summary.CalculateSettlements();
    }

    public static IReadOnlyCollection<ExpenseSettlement> CalculateSettlements(DateOnly month, IEnumerable<Transaction> transactions)
    {
        var summary = BuildMonthlySummary(month, transactions);
        return summary.CalculateSettlements();
    }
}
