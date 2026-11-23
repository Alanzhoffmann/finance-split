namespace FinanceSplit.Contracts.People.Commands;

public record DeleteIncomeRecordCommand(Guid Id, Guid PersonId);
