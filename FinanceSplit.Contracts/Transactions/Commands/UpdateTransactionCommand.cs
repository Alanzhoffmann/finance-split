using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using FinanceSplit.Contracts.Transactions.Enums;
using NetEscapades.EnumGenerators;

namespace FinanceSplit.Contracts.Transactions.Commands;

public record UpdateTransactionCommand(
    [property: JsonIgnore] Guid Id,
    string Name,
    DateOnly Month,
    decimal Value,
    Guid PaidByPersonId,
    SplitType SplitType,
    IReadOnlyCollection<Guid> SharedWithIds,
    SplitTransactionSetup? SplitSetup = null,
    RecurrenceTransactionSetup? RecurrenceSetup = null
);

[EnumExtensions]
public enum UpdateRecurrence
{
    [EnumMember(Value = "Only this transaction")]
    Transaction,

    [EnumMember(Value = "All future transactions")]
    Future,

    [EnumMember(Value = "All future and previous transactions")]
    All,
}
