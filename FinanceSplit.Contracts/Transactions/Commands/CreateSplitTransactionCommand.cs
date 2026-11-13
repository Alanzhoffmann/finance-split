using FinanceSplit.Contracts.Enums;

namespace FinanceSplit.Contracts.Transactions.Commands;

public record CreateSplitTransactionCommand(
    string Name,
    DateOnly Date,
    decimal Value,
    int NumberOfInstallments,
    Guid PaidByPersonId,
    SplitType SplitType,
    IReadOnlyCollection<Guid> SharedWithIds
);
