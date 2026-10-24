namespace FinanceSplit.Domain.Entities;

public sealed class Person : BaseEntity
{
    public required string Name { get; init; }

    public List<IncomeRecord> IncomeRecords { get; init; } = [];
    public List<Transaction> Transactions { get; init; } = [];
    public List<Transaction> SharedTransactions { get; init; } = [];
}
