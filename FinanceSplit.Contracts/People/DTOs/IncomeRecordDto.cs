using FinanceSplit.Contracts.People.Enums;

namespace FinanceSplit.Contracts.People.DTOs;

public record IncomeRecordDto(Guid Id, DateOnly Month, decimal Amount, IncomeType Type);
