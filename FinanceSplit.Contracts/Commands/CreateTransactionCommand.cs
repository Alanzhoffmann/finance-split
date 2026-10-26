using FinanceSplit.Contracts.Enums;

namespace FinanceSplit.Contracts.Commands;

public record CreateTransactionCommand(
    string Name,
    DateOnly Date,
    decimal Value,
    Guid PaidByPersonId,
    bool IsRecurring,
    SplitType SplitType,
    Guid[] SharedWithIds
);
