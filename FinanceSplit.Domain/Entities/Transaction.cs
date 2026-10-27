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

    public IReadOnlyDictionary<Person, decimal> CalculateShares() =>
        SplitType switch
        {
            SplitType.None => CalculateNoneShares(),
            SplitType.Even => CalculateEvenShares(),
            SplitType.Ratio => CalculateRatioShares(),
            _ => throw new InvalidOperationException($"Unsupported split type: {SplitType}"),
        };

    private Dictionary<Person, decimal> CalculateNoneShares() => new() { [SharedWith[0]] = Value };

    private Dictionary<Person, decimal> CalculateEvenShares()
    {
        var participants = SharedWith;
        var result = new Dictionary<Person, decimal>(participants.Count);

        // All participants except the first receive a share floored to 2dp.
        // The first participant (alphabetically) absorbs the remainder, rounding up naturally.
        var baseShare = decimal.Round(Value / participants.Count, 2, MidpointRounding.ToZero);
        var allocated = baseShare * (participants.Count - 1);
        result[participants[0]] = decimal.Round(
            Value - allocated,
            2,
            MidpointRounding.AwayFromZero
        );

        for (var i = 1; i < participants.Count; i++)
        {
            result[participants[i]] = baseShare;
        }

        return result;
    }

    private Dictionary<Person, decimal> CalculateRatioShares()
    {
        var incomePerPerson = SharedWith
            .Select(p => new { Person = p, Income = p.GetIncomeForMonth(Date) })
            .ToArray();

        if (incomePerPerson.Any(x => x.Income <= 0))
        {
            throw new InvalidOperationException(
                "Ratio split requires positive income for all participants in the month."
            );
        }

        var totalIncome = incomePerPerson.Sum(x => x.Income);

        // Highest earner is the adjustment person: their share is the remainder after
        // all others receive their proportional share floored to 2dp.
        var ordered = incomePerPerson
            .OrderByDescending(x => x.Income)
            .ThenBy(x => x.Person.Name, StringComparer.Ordinal)
            .ThenBy(x => x.Person.Id)
            .ToArray();

        var result = new Dictionary<Person, decimal>(SharedWith.Count);
        var allocated = 0m;

        for (var i = 1; i < ordered.Length; i++)
        {
            var share = decimal.Round(
                Value * ordered[i].Income / totalIncome,
                2,
                MidpointRounding.ToZero
            );
            result[ordered[i].Person] = share;
            allocated += share;
        }

        result[ordered[0].Person] = decimal.Round(
            Value - allocated,
            2,
            MidpointRounding.AwayFromZero
        );

        return result;
    }
}
