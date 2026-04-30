using FinanceSplit.Domain.Entities;
using FinanceSplit.Domain.Enums;

namespace FinanceSplit.Domain.ValueObjects;

public record SplitPay
{
    public SplitPay(IEnumerable<Person> people, SplitType splitType = SplitType.Even)
    {
        People = people;
        SplitType = splitType;
    }

    public IEnumerable<Person> People { get; private set; }
    public SplitType SplitType { get; private set; }

    public static SplitPay CreateNoneSplit(Person person) => new([person], SplitType.None);

    public static SplitPay CreateEvenSplit(IEnumerable<Person> people) => new(people, SplitType.Even);

    public static SplitPay CreateRatioSplit(IEnumerable<Person> people) => new(people, SplitType.Ratio);
}
