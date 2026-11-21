using FinanceSplit.Contracts.Transactions.DTOs;

namespace FinanceSplit.Contracts.Dashboard.DTOs;

public record GoalDto(
    string Name,
    DateOnly Month,
    decimal Amount,
    IReadOnlyList<TransactionShareDto> SharedWith,
    IReadOnlyList<TransactionShareDto> Completed
);
