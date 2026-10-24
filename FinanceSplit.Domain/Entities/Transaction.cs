using FinanceSplit.Contracts.Enums;

namespace FinanceSplit.Domain.Entities;

public class Transaction : BaseEntity
{
    public required string Name { get; init; }
    public required decimal Value { get; init; }
    public required DateTime Date { get; init; }

    public SplitType SplitType { get; init; } = SplitType.Even;
    public bool IsRecurring { get; init; }

    public required Person PaidByPerson { get; init; }
    public IReadOnlyList<Person> SharedWith { get; init; } = [];
}
