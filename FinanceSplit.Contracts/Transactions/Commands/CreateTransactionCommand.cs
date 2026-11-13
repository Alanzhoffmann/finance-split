using FinanceSplit.Contracts.Enums;

namespace FinanceSplit.Contracts.Transactions.Commands;

public record CreateTransactionCommand(
    string Name,
    DateOnly Date,
    decimal Value,
    Guid PaidByPersonId,
    SplitType SplitType,
    IReadOnlyCollection<Guid> SharedWithIds
);
