namespace FinanceSplit.Domain.ValueObjects;

public record ExpenseSettlement
{
    public ExpenseSettlement(Guid fromPersonId, Guid toPersonId, decimal amount)
    {
        if (fromPersonId == toPersonId)
        {
            throw new ArgumentException("Settlement requires two different people.");
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Settlement amount must be positive."
            );
        }

        FromPersonId = fromPersonId;
        ToPersonId = toPersonId;
        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
    }

    public Guid FromPersonId { get; }
    public Guid ToPersonId { get; }
    public decimal Amount { get; }
}
