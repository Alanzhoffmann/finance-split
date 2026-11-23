using FinanceSplit.Contracts.Transactions.Enums;
using FinanceSplit.Domain.Interfaces;

namespace FinanceSplit.Domain.Entities;

public class Goal : BaseEntity, ISharedAmount
{
    private readonly List<Transaction> _transactions = [];
    private List<Person> _sharedWith = [];

    public required string Name { get; init; }
    public required DateOnly InitialMonth { get; init; }
    public required DateOnly FinalMonth { get; init; }
    public decimal Value { get; init; }
    public SplitType SplitType { get; init; } = SplitType.None;
    public IReadOnlyList<Person> SharedWith
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
}
