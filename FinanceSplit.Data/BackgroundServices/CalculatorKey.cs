namespace FinanceSplit.Data.BackgroundServices;

public union CalculatorKey(
    IncomeRecordChanged,
    IncomeRecordDeleted,
    TransactionChanged,
    CalculateSummary
);

public record IncomeRecordChanged(DateOnly Month);

public record IncomeRecordDeleted(DateOnly Month);

public record TransactionChanged(DateOnly Month);

public record CalculateSummary(DateOnly Month);
