using FinanceSplit.Contracts.People.DTOs;

namespace FinanceSplit.Contracts.Dashboard.DTOs;

public record ExpenseSettlementDto(PersonDto From, PersonDto To, decimal Amount);
