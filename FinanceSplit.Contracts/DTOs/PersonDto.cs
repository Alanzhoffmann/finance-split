using System.Text.Json.Serialization;

namespace FinanceSplit.Contracts.DTOs;

public record PersonDto(Guid Id, string Name)
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<IncomeRecordResponse> Incomes { get; init; } = [];
}
