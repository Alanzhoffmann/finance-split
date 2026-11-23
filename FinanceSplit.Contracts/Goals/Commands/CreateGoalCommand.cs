using FinanceSplit.Contracts.Transactions.Enums;

namespace FinanceSplit.Contracts.Goals.Commands;

public record CreateGoalCommand(
    string Name,
    decimal Amount,
    DateOnly InitialMonth,
    DateOnly FinalMonth,
    SplitType SplitType,
    IReadOnlyCollection<Guid> SharedWithIds
);
