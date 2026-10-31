using FinanceSplit.Domain.ValueObjects;

namespace FinanceSplit.Domain.Entities;

public class MonthlyExpenseSummary : BaseEntity
{
    public MonthlyExpenseSummary(IEnumerable<Transaction> transactions)
    {
        ArgumentNullException.ThrowIfNull(transactions);

        var monthlyTransactions = transactions.ToArray();
        _transactions = monthlyTransactions;
    }

    private readonly IReadOnlyList<Transaction> _transactions;

    public IReadOnlyCollection<Transaction> Transactions => _transactions;

    public IReadOnlyDictionary<Guid, decimal> CalculateNetBalances()
    {
        var balances = new Dictionary<Guid, decimal>();

        foreach (var transaction in _transactions)
        {
            var shares = transaction.CalculateShares();

            AddToBalance(balances, transaction.PaidBy.Id, transaction.Value);

            foreach (var (person, share) in shares)
            {
                AddToBalance(balances, person.Id, -share);
            }
        }

        return balances
            .Where(x => x.Value != 0)
            .ToDictionary(
                x => x.Key,
                x => decimal.Round(x.Value, 2, MidpointRounding.AwayFromZero)
            );
    }

    public IReadOnlyCollection<ExpenseSettlement> CalculateSettlements()
    {
        var balances = CalculateNetBalances();
        var creditors = balances
            .Where(x => x.Value > 0)
            .Select(x => (PersonId: x.Key, Amount: x.Value))
            .OrderByDescending(x => x.Amount)
            .ThenBy(x => x.PersonId)
            .ToList();

        var debtors = balances
            .Where(x => x.Value < 0)
            .Select(x => (PersonId: x.Key, Amount: -x.Value))
            .OrderByDescending(x => x.Amount)
            .ThenBy(x => x.PersonId)
            .ToList();

        var settlements = new List<ExpenseSettlement>();
        var creditorIndex = 0;
        var debtorIndex = 0;

        while (creditorIndex < creditors.Count && debtorIndex < debtors.Count)
        {
            var creditor = creditors[creditorIndex];
            var debtor = debtors[debtorIndex];
            var settlementAmount = Math.Min(creditor.Amount, debtor.Amount);

            if (settlementAmount > 0)
            {
                settlements.Add(
                    new ExpenseSettlement(debtor.PersonId, creditor.PersonId, settlementAmount)
                );
            }

            creditor.Amount = decimal.Round(
                creditor.Amount - settlementAmount,
                2,
                MidpointRounding.AwayFromZero
            );
            debtor.Amount = decimal.Round(
                debtor.Amount - settlementAmount,
                2,
                MidpointRounding.AwayFromZero
            );

            creditors[creditorIndex] = creditor;
            debtors[debtorIndex] = debtor;

            if (creditor.Amount == 0)
            {
                creditorIndex++;
            }

            if (debtor.Amount == 0)
            {
                debtorIndex++;
            }
        }

        return settlements;
    }

    private static void AddToBalance(
        Dictionary<Guid, decimal> balances,
        Guid personId,
        decimal amount
    )
    {
        balances.TryGetValue(personId, out var current);
        balances[personId] = decimal.Round(current + amount, 2, MidpointRounding.AwayFromZero);
    }
}
