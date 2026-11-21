namespace FinanceSplit.Contracts.Dashboard.DTOs;

public record DashboardDto(MonthSummaryDto MonthSummary, IReadOnlyList<GoalDto> Goals);
