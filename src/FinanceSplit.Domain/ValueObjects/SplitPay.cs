using FinanceSplit.Contracts.Enums;
using FinanceSplit.Domain.Entities;

namespace FinanceSplit.Domain.ValueObjects;

public record SplitPay
{
    // Required for EF Core materialization
    private SplitPay() { }

    public SplitPay(IEnumerable<Person> people, SplitType splitType = SplitType.Even)
    {
        ArgumentNullException.ThrowIfNull(people);

        var participants = people.DistinctBy(p => p.Id).OrderBy(p => p.Name, StringComparer.Ordinal).ThenBy(p => p.Id).ToArray();

        if (participants.Length == 0)
        {
            throw new ArgumentException("At least one participant is required.", nameof(people));
        }

        if (splitType == SplitType.None && participants.Length != 1)
        {
            throw new ArgumentException("None split must contain exactly one participant.", nameof(people));
        }

        People = participants;
        SplitType = splitType;
    }

    public IEnumerable<Person> People { get; private set; }
    public SplitType SplitType { get; private set; }

    public static SplitPay CreateNoneSplit(Person person) => new([person], SplitType.None);

    public static SplitPay CreateEvenSplit(IEnumerable<Person> people) => new(people, SplitType.Even);

    public static SplitPay CreateRatioSplit(IEnumerable<Person> people) => new(people, SplitType.Ratio);

    public IReadOnlyDictionary<Person, decimal> CalculateShares(decimal amount, DateOnly month)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative.");
        }

        var participants = People.ToArray();

        return SplitType switch
        {
            SplitType.None => CalculateNoneShares(participants, amount),
            SplitType.Even => CalculateEvenShares(participants, amount),
            SplitType.Ratio => CalculateRatioShares(participants, amount, month),
            _ => throw new InvalidOperationException($"Unsupported split type: {SplitType}"),
        };
    }

    private static Dictionary<Person, decimal> CalculateNoneShares(Person[] participants, decimal amount) => new() { [participants[0]] = amount };

    private static Dictionary<Person, decimal> CalculateEvenShares(Person[] participants, decimal amount)
    {
        var result = new Dictionary<Person, decimal>(participants.Length);

        // All participants except the first receive a share floored to 2dp.
        // The first participant (alphabetically) absorbs the remainder, rounding up naturally.
        var baseShare = decimal.Round(amount / participants.Length, 2, MidpointRounding.ToZero);
        var allocated = baseShare * (participants.Length - 1);
        result[participants[0]] = decimal.Round(amount - allocated, 2, MidpointRounding.AwayFromZero);

        for (var i = 1; i < participants.Length; i++)
        {
            result[participants[i]] = baseShare;
        }

        return result;
    }

    private static Dictionary<Person, decimal> CalculateRatioShares(Person[] participants, decimal amount, DateOnly month)
    {
        var salaryByPerson = participants.Select(p => new { Person = p, Salary = p.GetSalaryForMonth(month) }).ToArray();

        if (salaryByPerson.Any(x => x.Salary <= 0))
        {
            throw new InvalidOperationException("Ratio split requires positive salary for all participants in the month.");
        }

        var totalSalary = salaryByPerson.Sum(x => x.Salary);

        // Highest earner is the adjustment person: their share is the remainder after
        // all others receive their proportional share floored to 2dp.
        var ordered = salaryByPerson.OrderByDescending(x => x.Salary).ThenBy(x => x.Person.Name, StringComparer.Ordinal).ThenBy(x => x.Person.Id).ToArray();

        var result = new Dictionary<Person, decimal>(participants.Length);
        var allocated = 0m;

        for (var i = 1; i < ordered.Length; i++)
        {
            var share = decimal.Round(amount * ordered[i].Salary / totalSalary, 2, MidpointRounding.ToZero);
            result[ordered[i].Person] = share;
            allocated += share;
        }

        result[ordered[0].Person] = decimal.Round(amount - allocated, 2, MidpointRounding.AwayFromZero);

        return result;
    }
}
