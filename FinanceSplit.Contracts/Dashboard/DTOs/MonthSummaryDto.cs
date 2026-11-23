namespace FinanceSplit.Contracts.Dashboard.DTOs;

public record MonthSummaryDto(
    IReadOnlyList<PersonSummaryDto> People,
    IReadOnlyList<ExpenseSettlementDto> Settlements
);
