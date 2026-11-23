using FinanceSplit.Contracts.Transactions.DTOs;

namespace FinanceSplit.Contracts.Goals.DTOs;

public record GoalDto(
    Guid Id,
    string Name,
    DateOnly Month,
    decimal Amount,
    IReadOnlyList<TransactionShareDto> SharedWith,
    IReadOnlyList<TransactionShareDto> Completed
);
