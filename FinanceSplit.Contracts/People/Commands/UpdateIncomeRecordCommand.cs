using System.Text.Json.Serialization;
using FinanceSplit.Contracts.People.Enums;

namespace FinanceSplit.Contracts.People.Commands;

public record UpdateIncomeRecordCommand(
    [property: JsonIgnore] Guid Id,
    [property: JsonIgnore] Guid PersonId,
    decimal Amount,
    DateOnly Month,
    IncomeType Type
);
