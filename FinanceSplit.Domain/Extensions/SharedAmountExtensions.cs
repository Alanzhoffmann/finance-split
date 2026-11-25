using FinanceSplit.Contracts.Transactions.Enums;
using FinanceSplit.Domain.Entities;
using FinanceSplit.Domain.Interfaces;

namespace FinanceSplit.Domain.Extensions;

public static class SharedAmountExtensions
{
    public static IReadOnlyDictionary<Person, decimal> CalculateShares(
        this ISharedAmount sharedAmount,
        params IEnumerable<DateOnly> months
    ) =>
        sharedAmount.SplitType switch
        {
            SplitType.None => CalculateNoneShares(sharedAmount),
            SplitType.Even => CalculateEvenShares(sharedAmount),
            SplitType.Ratio => CalculateRatioShares(sharedAmount, months),
            _ => throw new InvalidOperationException(
                $"Unsupported split type: {sharedAmount.SplitType}"
            ),
        };

    private static Dictionary<Person, decimal> CalculateNoneShares(ISharedAmount sharedAmount) =>
        new() { [sharedAmount.SharedWith[0]] = sharedAmount.Value };

    private static Dictionary<Person, decimal> CalculateEvenShares(ISharedAmount sharedAmount)
    {
        var sharedWith = sharedAmount.SharedWith;
        var value = sharedAmount.Value;

        var result = new Dictionary<Person, decimal>(sharedWith.Count);

        // All participants except the first receive a share floored to 2dp.
        // The first participant (alphabetically) absorbs the remainder, rounding up naturally.
        var baseShare = decimal.Round(value / sharedWith.Count, 2, MidpointRounding.ToZero);
        var allocated = baseShare * (sharedWith.Count - 1);
        result[sharedWith[0]] = decimal.Round(value - allocated, 2, MidpointRounding.AwayFromZero);

        for (var i = 1; i < sharedWith.Count; i++)
        {
            result[sharedWith[i]] = baseShare;
        }

        return result;
    }

    private static Dictionary<Person, decimal> CalculateRatioShares(
        ISharedAmount sharedAmount,
        IEnumerable<DateOnly> months
    )
    {
        var sharedWith = sharedAmount.SharedWith;
        var value = sharedAmount.Value;

        var incomePerPerson = sharedWith
            .Select(p => new { Person = p, Income = months.Sum(m => p.GetIncomeForMonth(m)) })
            .ToArray();

        if (incomePerPerson.Any(x => x.Income < 0))
        {
            throw new InvalidOperationException(
                "Ratio split requires positive income for all participants in the month."
            );
        }

        var totalIncome = incomePerPerson.Sum(x => x.Income);

        // if no incomes available, return even split
        if (totalIncome == 0)
        {
            return CalculateEvenShares(sharedAmount);
        }

        // Highest earner is the adjustment person: their share is the remainder after
        // all others receive their proportional share floored to 2dp.
        var ordered = incomePerPerson
            .OrderByDescending(x => x.Income)
            .ThenBy(x => x.Person.Name, StringComparer.Ordinal)
            .ThenBy(x => x.Person.Id)
            .ToArray();

        var result = new Dictionary<Person, decimal>(sharedWith.Count);
        var allocated = 0m;

        for (var i = 1; i < ordered.Length; i++)
        {
            var share = decimal.Round(
                value * ordered[i].Income / totalIncome,
                2,
                MidpointRounding.ToZero
            );
            result[ordered[i].Person] = share;
            allocated += share;
        }

        result[ordered[0].Person] = decimal.Round(
            value - allocated,
            2,
            MidpointRounding.AwayFromZero
        );

        return result;
    }
}
