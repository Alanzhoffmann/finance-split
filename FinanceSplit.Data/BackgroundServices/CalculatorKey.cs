namespace FinanceSplit.Data.BackgroundServices;

public union CalculatorKey(IncomeRecordChanged, IncomeRecordDeleted, TransactionChanged);

public record IncomeRecordChanged(DateOnly Month);

public record IncomeRecordDeleted(DateOnly Month);

public record TransactionChanged(DateOnly Month);
