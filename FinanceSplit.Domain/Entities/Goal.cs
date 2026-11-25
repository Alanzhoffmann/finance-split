using FinanceSplit.Contracts.Transactions.Enums;
using FinanceSplit.Domain.Interfaces;

namespace FinanceSplit.Domain.Entities;

public class Goal : BaseEntity, ISharedAmount
{
    private readonly List<Transaction> _transactions = [];
    private List<Person> _sharedWith = [];
    private string? _name;
    private DateOnly _initialMonth;
    private DateOnly _finalMonth;
    private decimal _amount;
    private SplitType _splitType = SplitType.None;

    public required string Name
    {
        get => _name ?? "unset";
        init => _name = value;
    }
    public required DateOnly InitialMonth
    {
        get => _initialMonth;
        init => _initialMonth = value;
    }
    public required DateOnly FinalMonth
    {
        get => _finalMonth;
        init => _finalMonth = value;
    }
    public decimal Value
    {
        get => _amount;
        init => _amount = value;
    }
    public SplitType SplitType
    {
        get => _splitType;
        init => _splitType = value;
    }

    public required IReadOnlyList<Person> SharedWith
    {
        get => _sharedWith;
        init => _sharedWith = [.. value];
    }

    public IReadOnlyList<Transaction> Transactions => _transactions;

    public IEnumerable<DateOnly> Months
    {
        get
        {
            for (DateOnly i = InitialMonth; i <= FinalMonth; i = i.AddMonths(1))
            {
                yield return i;
            }
        }
    }

    public void Update(
        string name,
        decimal amount,
        DateOnly initialMonth,
        DateOnly finalMonth,
        SplitType splitType
    )
    {
        _name = name;
        _amount = amount;
        _initialMonth = initialMonth;
        _finalMonth = finalMonth;
        _splitType = splitType;
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
