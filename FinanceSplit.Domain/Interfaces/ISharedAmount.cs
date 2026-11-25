using FinanceSplit.Contracts.Transactions.Enums;
using FinanceSplit.Domain.Entities;

namespace FinanceSplit.Domain.Interfaces;

public interface ISharedAmount
{
    decimal Value { get; }
    SplitType SplitType { get; }
    IReadOnlyList<Person> SharedWith { get; }
}
