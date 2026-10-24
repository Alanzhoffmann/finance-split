using FinanceSplit.Contracts.Enums;

namespace FinanceSplit.Contracts.DTOs;

public record IncomeRecordResponse(Guid Id, DateTime Month, decimal Amount, IncomeType Type);
