namespace FinanceSplit.Data.BackgroundServices;

public union CalculatorKey(
    IncomeRecordChanged,
    IncomeRecordDeleted,
    TransactionChanged,
    RecurrenceChanged
);

public record IncomeRecordChanged(DateOnly Month);

public record IncomeRecordDeleted(DateOnly Month);

public record TransactionChanged(DateOnly Month);

public record RecurrenceChanged(DateOnly Month);
