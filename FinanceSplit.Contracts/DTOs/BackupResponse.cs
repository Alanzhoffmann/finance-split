using System.Text.Json.Serialization;
using FinanceSplit.Contracts.Enums;

namespace FinanceSplit.Contracts.DTOs;

public record BackupResponse(
    string FormatVersion,
    DateTimeOffset ExportedAtUtc,
    BackupPayload Data
);

public record BackupPayload(
    IReadOnlyList<BackupPerson> People,
    IReadOnlyList<BackupTransaction> Transactions
);

public record BackupPerson(Guid Id, string Name, IReadOnlyList<BackupIncomeRecord> Salaries);

public record BackupIncomeRecord(
    Guid Id,
    DateOnly Month,
    decimal Amount,
    [property: JsonConverter(typeof(JsonStringEnumConverter<IncomeType>))] IncomeType Type
);

public record BackupTransaction(
    Guid Id,
    string Name,
    decimal Value,
    DateOnly Date,
    [property: JsonConverter(typeof(JsonStringEnumConverter<SplitType>))] SplitType SplitType,
    Guid PaidByPersonId,
    bool IsRecurring,
    IReadOnlyList<Guid> SharedWith
);
