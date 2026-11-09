namespace FinanceSplit.Domain.Entities;

public class Recurrence : BaseEntity
{
    private readonly List<Transaction> _transactions = [];

    private Recurrence() { }

    public RecurrenceType Type { get; init; }
    public decimal Amount { get; init; }
    public int? NumberOfRecurrences { get; init; }
    public IReadOnlyList<Transaction> Transactions
    {
        get => _transactions;
        init => _transactions = [.. value];
    }

    public DateOnly? LastMonth { get; init; }
    public bool Ended { get; private set; } = false;

    public static Recurrence ForPeriodOfTime(decimal amount, DateOnly lastMonth)
    {
        return new()
        {
            Type = RecurrenceType.PeriodOfTime,
            Amount = amount,
            LastMonth = lastMonth,
        };
    }

    public static Recurrence ForNumberOfTimes(decimal amount, int numberOfTimes)
    {
        return new()
        {
            Type = RecurrenceType.NumberOfTimes,
            Amount = amount,
            NumberOfRecurrences = numberOfTimes,
        };
    }

    public static Recurrence SplitInstallments(Transaction transaction, int numberOfInstallments)
    {
        var installments = Math.Round(
            transaction.Value / numberOfInstallments,
            2,
            MidpointRounding.ToZero
        );

        IReadOnlyList<Transaction> transactions =
        [
            .. Enumerable
                .Range(0, numberOfInstallments)
                .Select(i => new Transaction()
                {
                    Name = $"{transaction.Name} {i + 1}/{numberOfInstallments}",
                    Month = transaction.Month.AddMonths(i),
                    PaidBy = transaction.PaidBy,
                    Value = installments,
                    SharedWith = transaction.SharedWith,
                    SplitType = transaction.SplitType,
                }),
        ];

        // make sure all transactions sum up to the original amount by adding the remainder to the last one
        transactions[^1].AddValue(transaction.Value - transactions.Sum(t => t.Value));

        return new()
        {
            NumberOfRecurrences = numberOfInstallments,
            Amount = transaction.Value,
            Transactions = transactions,
            Ended = true,
        };
    }

    public static Recurrence Forever(decimal amount)
    {
        return new() { Type = RecurrenceType.Forever, Amount = amount };
    }

    public void EndRecurrence()
    {
        Ended = true;
    }
}
