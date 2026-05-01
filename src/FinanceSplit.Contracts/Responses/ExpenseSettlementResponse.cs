namespace FinanceSplit.Contracts.Responses;

public record ExpenseSettlementResponse(PersonResponse From, PersonResponse To, decimal Amount);
