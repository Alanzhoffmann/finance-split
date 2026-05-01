namespace FinanceSplit.Domain.ValueObjects;

/// <summary>
/// Represents a monthly salary. The date is always normalised to the first of the month,
/// and the salary applies to that month and every following month until a newer one is added.
/// </summary>
public record Salary
{
    // Required for EF Core materialization
    private Salary() { }

    public Salary(DateOnly date, decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Salary amount must be positive.");
        }

        Date = new DateOnly(date.Year, date.Month, 1);
        Amount = amount;
    }

    public DateOnly Date { get; }
    public decimal Amount { get; }

    public static Salary ForMonth(int year, int month, decimal amount) => new(new DateOnly(year, month, 1), amount);
}
