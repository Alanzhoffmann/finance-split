using FinanceSplit.Contracts.Enums;

namespace FinanceSplit.Domain.Entities;

public class IncomeRecord : BaseEntity
{
    public required DateTime Month { get; init; }
    public required decimal Amount { get; init; }
    public IncomeType Type { get; init; } = IncomeType.Salary;

    public static IncomeRecord ForMonth(
        int month,
        int year,
        decimal amount,
        IncomeType type = IncomeType.Salary
    ) =>
        new()
        {
            Amount = amount,
            Month = new DateTime(year, month, 1),
            Type = type,
        };
}
