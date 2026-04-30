using FinanceSplit.Domain.ValueObjects;

namespace FinanceSplit.Domain.Entities;

public class Person : BaseEntity
{
    public Person(string name)
    {
        Name = name;
    }

    private readonly IList<Salary> _salaries = [];

    public string Name { get; private set; }
    public IEnumerable<Salary> Salaries => _salaries;

    public void AddSalary(Salary salary) => _salaries.Add(salary);

    public void UpdateName(string name) => Name = name;
}
