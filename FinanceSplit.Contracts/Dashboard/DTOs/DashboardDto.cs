using System.Text.Json.Serialization;
using FinanceSplit.Contracts.People.DTOs;

namespace FinanceSplit.Contracts.Dashboard.DTOs;

public record DashboardDto(
    IReadOnlyList<PersonSummaryDto> People,
    IReadOnlyList<ExpenseSettlementDto> Settlements
);

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

public record ExpenseSettlementDto(PersonDto From, PersonDto To, decimal Amount);
