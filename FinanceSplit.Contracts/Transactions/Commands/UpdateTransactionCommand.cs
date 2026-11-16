using System.Text.Json.Serialization;
using FinanceSplit.Contracts.Transactions.Enums;

namespace FinanceSplit.Contracts.Transactions.Commands;

public record UpdateTransactionCommand(
    [property: JsonIgnore] Guid Id,
    string Name,
    DateOnly Date,
    decimal Value,
    Guid PaidByPersonId,
    SplitType SplitType,
    IReadOnlyCollection<Guid> SharedWithIds,
    SplitTransactionSetup? SplitSetup = null,
    RecurrenceTransactionSetup? RecurrenceSetup = null
);
