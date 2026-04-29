namespace FinanceSplit.Domain.Entities;

public class Transaction : BaseEntity
{
    public Transaction(string title, decimal amount, Person paidBy)
    {
        Title = title;
        Amount = amount;
        PaidBy = paidBy;
    }

    public string Title { get; private set; } = string.Empty;
    public Person PaidBy { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime Date { get; private set; }
    public string Description { get; private set; } = string.Empty;
}