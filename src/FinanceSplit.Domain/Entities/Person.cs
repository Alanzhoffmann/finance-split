using FinanceSplit.Domain.ValueObjects;

namespace FinanceSplit.Domain.Entities;

public class Person : BaseEntity
{
    public Person(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }

    private readonly IList<Salary> _salaries = [];

    public string Name { get; private set; }
    public IEnumerable<Salary> Salaries => _salaries;

    public void AddSalary(Salary salary) => _salaries.Add(salary);

    public decimal GetSalaryForMonth(DateOnly month)
    {
        var monthStart = new DateOnly(month.Year, month.Month, 1);

        return _salaries.Where(s => s.Date <= monthStart).OrderByDescending(s => s.Date).Select(s => s.Amount).FirstOrDefault();
    }

    public void UpdateName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }
}
