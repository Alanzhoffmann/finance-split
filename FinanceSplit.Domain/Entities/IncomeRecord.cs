using FinanceSplit.Contracts.Enums;

namespace FinanceSplit.Domain.Entities;

public class IncomeRecord : BaseEntity
{
    private DateOnly _month;
    private decimal _amount;
    private IncomeType _type = IncomeType.Salary;

    public required DateOnly Month
    {
        get => _month;
        init => _month = value;
    }
    public required decimal Amount
    {
        get => _amount;
        init => _amount = value;
    }
    public IncomeType Type
    {
        get => _type;
        init => _type = value;
    }

    public void Update(decimal amount, DateOnly month, IncomeType type)
    {
        _amount = amount;
        _month = month;
        _type = type;
    }

    public static IncomeRecord ForMonth(
        int month,
        int year,
        decimal amount,
        IncomeType type = IncomeType.Salary
    ) =>
        new()
        {
            Amount = amount,
            Month = new DateOnly(year, month, 1),
            Type = type,
        };
}
