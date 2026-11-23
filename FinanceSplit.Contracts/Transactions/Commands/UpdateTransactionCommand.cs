using System.Text.Json.Serialization;
using FinanceSplit.Contracts.Transactions.Enums;

namespace FinanceSplit.Contracts.Transactions.Commands;

public record UpdateTransactionCommand(
    [property: JsonIgnore] Guid Id,
    string Name,
    DateOnly Month,
    decimal Value,
    Guid PaidByPersonId,
    SplitType SplitType,
    IReadOnlyCollection<Guid> SharedWithIds
);
