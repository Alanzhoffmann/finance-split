using FinanceSplit.Contracts.Enums;

namespace FinanceSplit.Contracts.DTOs;

public record IncomeRecordResponse(Guid Id, DateOnly Month, decimal Amount, IncomeType Type);
