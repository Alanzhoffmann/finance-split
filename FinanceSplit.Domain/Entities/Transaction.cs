using FinanceSplit.Contracts.Enums;

namespace FinanceSplit.Domain.Entities;

public class Transaction : BaseEntity
{
    private string? _name;
    private decimal _value;
    private DateOnly _date;
    private bool _isRecurring;
    private Person? _paidBy;
    private IReadOnlyList<Person> _sharedWith = [];
    private SplitType _splitType = SplitType.Even;

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
    public required DateOnly Date
    {
        get => _date;
        init => _date = value;
    }

    public SplitType SplitType
    {
        get => _splitType;
        init => _splitType = value;
    }
    public bool IsRecurring
    {
        get => _isRecurring;
        init => _isRecurring = value;
    }

    public required Person PaidBy
    {
        get => _paidBy ?? throw new NullReferenceException("_paidBy is null");
        init => _paidBy = value;
    }
    public IReadOnlyList<Person> SharedWith
    {
        get => _sharedWith;
        init => _sharedWith = value;
    }

    public void Update(
        string name,
        DateOnly date,
        decimal value,
        bool isRecurring,
        SplitType splitType
    )
    {
        _name = name;
        _date = date;
        _value = value;
        _isRecurring = isRecurring;
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
}
