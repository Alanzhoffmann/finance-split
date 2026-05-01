using FinanceSplit.Domain.Entities;

namespace FinanceSplit.Domain.ValueObjects;

public record ExpenseSettlement
{
    public ExpenseSettlement(Person from, Person to, decimal amount)
    {
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);

        if (from.Id == to.Id)
        {
            throw new ArgumentException("Settlement requires two different people.");
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Settlement amount must be positive.");
        }

        From = from;
        To = to;
        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
    }

    public Person From { get; }
    public Person To { get; }
    public decimal Amount { get; }
}
