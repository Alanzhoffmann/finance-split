using FinanceSplit.Contracts.Transactions.DTOs;
using FinanceSplit.Contracts.Transactions.Enums;

namespace FinanceSplit.Contracts.Goals.DTOs;

public record GoalDto(
    Guid Id,
    string Name,
    DateOnly InitialMonth,
    DateOnly FinalMonth,
    decimal Amount,
    SplitType SplitType,
    IReadOnlyList<TransactionShareDto> SharedWith,
    IReadOnlyList<TransactionShareDto> Completed
);
