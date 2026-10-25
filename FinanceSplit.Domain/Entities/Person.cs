namespace FinanceSplit.Domain.Entities;

public sealed class Person : BaseEntity
{
    private string? _name;

    public required string Name
    {
        get => _name ?? throw new NullReferenceException("_name is null");
        init
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _name = value;
        }
    }

    public List<IncomeRecord> IncomeRecords { get; init; } = [];
    public List<Transaction> Transactions { get; init; } = [];
    public List<Transaction> SharedTransactions { get; init; } = [];

    public void UpdateName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _name = name;
    }
}
