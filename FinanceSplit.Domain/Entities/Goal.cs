using FinanceSplit.Contracts.Transactions.Enums;
using FinanceSplit.Domain.Interfaces;

namespace FinanceSplit.Domain.Entities;

public class Goal : BaseEntity, ISharedAmount
{
    public required string Name { get; init; }
    public required DateOnly Month { get; init; }
    public decimal Value { get; init; }
    public SplitType SplitType { get; init; } = SplitType.None;
    public IReadOnlyList<Person> SharedWith { get; init; } = [];
}
