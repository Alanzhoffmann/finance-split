using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using FinanceSplit.Contracts.Transactions.Enums;
using NetEscapades.EnumGenerators;

namespace FinanceSplit.Contracts.Transactions.Commands;

public record UpdateRecurringTransactionCommand(
    [property: JsonIgnore] Guid RecurrenceId,
    [property: JsonIgnore] Guid? TransactionId,
    string Name,
    DateOnly Month,
    decimal Value,
    Guid PaidByPersonId,
    SplitType SplitType,
    IReadOnlyCollection<Guid> SharedWithIds,
    RecurrenceTransactionSetup RecurrenceSetup
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
