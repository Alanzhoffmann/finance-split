using System.Text.Json.Serialization;

namespace FinanceSplit.Contracts.Dashboard.DTOs;

public record PersonSummaryDto(
    string Name,
    decimal Income,
    decimal SharedExpenses,
    decimal PersonalExpenses,
    decimal TotalPaid
)
{
    [JsonIgnore]
    public decimal TotalShared => SharedExpenses + PersonalExpenses;

    [JsonIgnore]
    public decimal Leftover => Income - TotalShared;
}
