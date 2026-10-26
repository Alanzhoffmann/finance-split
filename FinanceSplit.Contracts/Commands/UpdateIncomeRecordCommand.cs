using System.Text.Json.Serialization;
using FinanceSplit.Contracts.Enums;

namespace FinanceSplit.Contracts.Commands;

public record UpdateIncomeRecordCommand(
    [property: JsonIgnore] Guid Id,
    [property: JsonIgnore] Guid PersonId,
    decimal Amount,
    DateOnly Month,
    IncomeType Type
);
