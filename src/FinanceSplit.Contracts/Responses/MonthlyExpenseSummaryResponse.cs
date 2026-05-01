namespace FinanceSplit.Contracts.Responses;

public record MonthlyExpenseSummaryResponse(
    Guid Id,
    DateOnly Month,
    IReadOnlyCollection<TransactionResponse> Transactions,
    IReadOnlyDictionary<PersonResponse, decimal> NetBalances,
    IReadOnlyCollection<ExpenseSettlementResponse> Settlements
);
