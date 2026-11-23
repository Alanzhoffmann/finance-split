using System.Text.Json.Serialization;

namespace FinanceSplit.Contracts.People.DTOs;

public record PersonDto(Guid Id, string Name)
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<IncomeRecordDto> Incomes { get; init; } = [];
}
