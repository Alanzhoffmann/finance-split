using System.Text.Json.Serialization;

namespace FinanceSplit.Contracts.DTOs;

public record DashboardResponse(
    IReadOnlyList<DashboardPersonResponse> People,
    IReadOnlyList<ExpenseSettlementResponse> Settlements
);

public record DashboardPersonResponse(
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

public record ExpenseSettlementResponse(string FromName, string ToName, decimal Amount);
