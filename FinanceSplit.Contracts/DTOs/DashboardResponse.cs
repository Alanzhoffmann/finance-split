namespace FinanceSplit.Contracts.DTOs;

public record DashboardResponse(IReadOnlyList<DashboardPersonResponse> People);

public record DashboardPersonResponse(
    string Name,
    decimal Income,
    decimal TotalExpenses,
    decimal TotalPaid
);
