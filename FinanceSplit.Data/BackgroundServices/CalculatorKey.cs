namespace FinanceSplit.Data.BackgroundServices;

public union CalculatorKey(IncomeRecordChanged, TransactionChanged);

public record IncomeRecordChanged(DateOnly Month);

public record TransactionChanged(DateOnly Month);
