using FinanceSplit.Contracts.Enums;

namespace FinanceSplit.Domain.Entities;

public class Transaction : BaseEntity
{
    public required string Name { get; init; }
    public required decimal Value { get; init; }
    public required DateOnly Date { get; init; }

    public SplitType SplitType { get; init; } = SplitType.Even;
    public bool IsRecurring { get; init; }

    public required Person PaidBy { get; init; }
    public IReadOnlyList<Person> SharedWith { get; init; } = [];
}
