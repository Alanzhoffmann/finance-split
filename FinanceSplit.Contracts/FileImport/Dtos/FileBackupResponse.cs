using System.Text.Json.Serialization;
using FinanceSplit.Contracts.People.Enums;
using FinanceSplit.Contracts.Transactions.Enums;

namespace FinanceSplit.Contracts.FileImport.DTOs;

public record FileBackupResponse(
    string FormatVersion,
    DateTimeOffset ExportedAtUtc,
    FilePayload Data
);

public record FilePayload(
    IReadOnlyList<FilePerson> People,
    IReadOnlyList<FileTransaction> Transactions
);

public record FilePerson(Guid Id, string Name, IReadOnlyList<FileIncomeRecord> Salaries);

public record FileIncomeRecord(
    Guid Id,
    DateOnly Month,
    decimal Amount,
    [property: JsonConverter(typeof(JsonStringEnumConverter<IncomeType>))] IncomeType Type
);

public record FileTransaction(
    Guid Id,
    string Name,
    decimal Value,
    DateOnly Date,
    [property: JsonConverter(typeof(JsonStringEnumConverter<SplitType>))] SplitType SplitType,
    Guid PaidByPersonId,
    IReadOnlyList<Guid> SharedWith
);
