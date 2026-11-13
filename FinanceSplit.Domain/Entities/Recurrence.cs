namespace FinanceSplit.Domain.Entities;

public class Recurrence : Transaction
{
    private readonly List<Transaction> _transactions = [];

    public RecurrenceType Type { get; init; }
    public int? NumberOfRecurrences { get; init; }
    public IReadOnlyList<Transaction> Transactions
    {
        get => _transactions;
        init => _transactions = [.. value];
    }

    public DateOnly? LastMonth { get; init; }
    public bool Ended { get; private set; } = false;

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
            Name = transaction.Name,
            Month = transaction.Month,
            PaidBy = transaction.PaidBy,
            SharedWith = transaction.SharedWith,
            Value = transaction.Value,
            SplitType = transaction.SplitType,
            NumberOfRecurrences = numberOfInstallments,
            Transactions = transactions,
            Ended = true,
        };
    }

    public void EndRecurrence()
    {
        Ended = true;
    }

    private bool IsForMonth(DateOnly month) =>
        Type is RecurrenceType.Forever
        || LastMonth >= month
        || (NumberOfRecurrences is not null && Month.AddMonths(NumberOfRecurrences.Value) >= month);

    public bool CanAddNewTransaction(DateOnly month)
    {
        if (!IsForMonth(month))
        {
            return false;
        }

        // Check if that transaction has been added already
        return !Transactions.Any(t => t.Month == month);
    }

    public Transaction? GetTransactionForMonth(DateOnly month)
    {
        var transactionForMonth = Transactions.FirstOrDefault(t => t.Month == month);
        if (transactionForMonth is not null)
        {
            return transactionForMonth;
        }

        if (!CanAddNewTransaction(month))
        {
            return null;
        }

        return new Transaction
        {
            Month = month,
            Name = Name,
            PaidBy = PaidBy,
            Value = Value,
            Recurrence = this,
            SharedWith = SharedWith,
            SplitType = SplitType,
        };
    }
}
