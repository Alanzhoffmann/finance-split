namespace FinanceSplit.Contracts.Responses;

public record PersonResponse(Guid Id, string Name, IReadOnlyCollection<SalaryResponse> Salaries);
