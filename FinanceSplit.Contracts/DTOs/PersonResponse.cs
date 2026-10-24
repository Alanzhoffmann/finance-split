namespace FinanceSplit.Contracts.DTOs;

public record PersonResponse(Guid Id, string Name, IReadOnlyList<IncomeRecordResponse> Incomes);
