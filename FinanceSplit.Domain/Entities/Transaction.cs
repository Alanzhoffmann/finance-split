using FinanceSplit.Contracts.Transactions.Enums;
using FinanceSplit.Domain.Interfaces;

namespace FinanceSplit.Domain.Entities;

public class Transaction : BaseEntity, ISharedAmount
{
    private string? _name;
    private decimal _value;
    private DateOnly _month;
    private Person? _paidBy;
    private List<Person> _sharedWith = [];
    private SplitType _splitType = SplitType.Even;
    private Goal? _goal;

    public required string Name
    {
        get => _name ?? throw new NullReferenceException("_name is null");
        init => _name = value;
    }

    public required decimal Value
    {
        get => _value;
        init => _value = value;
    }

    public required DateOnly Month
    {
        get => _month;
        init => _month = value.Day == 1 ? value : new(value.Year, value.Month, 1);
    }

    public SplitType SplitType
    {
        get => _splitType;
        init => _splitType = value;
    }

    public required Person PaidBy
    {
        get => _paidBy ?? throw new NullReferenceException("_paidBy is null");
        init => _paidBy = value;
    }

    public IReadOnlyList<Person> SharedWith
    {
        get => _sharedWith;
        init => _sharedWith = [.. value];
    }

    public Recurrence? Recurrence { get; init; }
    public Goal? Goal
    {
        get => _goal;
        init => _goal = value;
    }

    public void Update(string name, DateOnly month, decimal value, SplitType splitType)
    {
        _name = name;
        _month = month;
        _value = value;
        _splitType = splitType;
    }

    public void UpdatePaidBy(Person paidByPerson)
    {
        if (paidByPerson.Id == PaidBy.Id)
        {
            return;
        }

        _paidBy = paidByPerson;
    }

    public void UpdateSharedWith(List<Person> sharedWith)
    {
        if (sharedWith.Select(p => p.Id).SequenceEqual(SharedWith.Select(p => p.Id)))
        {
            return;
        }

        _sharedWith = sharedWith;
    }

    public void UpdateGoal(Goal? goal)
    {
        _goal = goal;
    }

    public void AddValue(decimal value) => _value += value;
}
