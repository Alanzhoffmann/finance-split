using System.Text.Json.Serialization;
using FinanceSplit.Contracts.Transactions.Enums;

namespace FinanceSplit.Contracts.Goals.Commands;

public record UpdateGoalCommand(
    [property: JsonIgnore] Guid Id,
    string Name,
    decimal Amount,
    DateOnly InitialMonth,
    DateOnly FinalMonth,
    SplitType SplitType,
    IReadOnlyCollection<Guid> SharedWithIds
);
