using System.Text.Json.Serialization;
using FinanceSplit.Contracts.Enums;

namespace FinanceSplit.Contracts.Requests;

public record ImportBackupRequest(string FormatVersion, DateTimeOffset ExportedAtUtc, ImportPayload Data);

public record ImportPayload(List<ImportPerson> People, List<ImportTransaction> Transactions);

public record ImportPerson(Guid Id, string Name, List<ImportSalaryRecord> Salaries);

public record ImportSalaryRecord(Guid Id, int Month, int Year, decimal Amount);

public record ImportTransaction(
    Guid Id,
    string Name,
    decimal Value,
    int Month,
    int Year,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] SplitType SplitType,
    Guid PaidByPersonId,
    bool IsRecurring,
    List<ImportTransactionResponsibility> Responsibilities
);

public record ImportTransactionResponsibility(Guid Id, Guid PersonId);
