using System.Text.Json.Serialization;
using FinanceSplit.Contracts.Enums;

namespace FinanceSplit.Contracts.Commands;

public record UpdateTransactionCommand(
    [property: JsonIgnore] Guid Id,
    string Name,
    DateOnly Date,
    decimal Value,
    Guid PaidByPersonId,
    bool IsRecurring,
    SplitType SplitType,
    Guid[] SharedWithIds
);
