namespace FinanceSplit.Contracts.Commands;

public record DeleteIncomeRecordCommand(Guid Id, Guid PersonId);
