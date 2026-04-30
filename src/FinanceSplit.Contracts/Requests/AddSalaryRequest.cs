namespace FinanceSplit.Contracts.Requests;

public record AddSalaryRequest(DateOnly Date, decimal Amount);
