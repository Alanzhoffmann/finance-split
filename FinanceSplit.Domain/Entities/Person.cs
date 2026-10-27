using FinanceSplit.Contracts.Enums;

namespace FinanceSplit.Domain.Entities;

public sealed class Person : BaseEntity
{
    private string? _name;

    public required string Name
    {
        get => _name ?? throw new NullReferenceException("_name is null");
        init
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _name = value;
        }
    }

    public List<IncomeRecord> IncomeRecords { get; init; } = [];
    public List<Transaction> Transactions { get; init; } = [];
    public List<Transaction> SharedTransactions { get; init; } = [];

    public void UpdateName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _name = name;
    }

    public decimal GetIncomeForMonth(DateOnly date)
    {
        var monthStart = new DateOnly(date.Year, date.Month, 1);

        var monthIncomes = IncomeRecords.Where(i => i.Month == monthStart).ToList();

        if (monthIncomes.Count == 0)
        {
            // if no incomes for a given month, get all salaries from the first previous month available
            return IncomeRecords
                    .Where(i => i.Month < monthStart && i.Type == IncomeType.Salary)
                    .GroupBy(i => i.Month)
                    .OrderByDescending(g => g.Key)
                    .FirstOrDefault()
                    ?.Sum(i => i.Amount)
                ?? 0;
        }

        return monthIncomes.Sum(i => i.Amount);
    }
}
