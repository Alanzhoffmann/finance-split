using FinanceSplit.Domain.ValueObjects;

namespace FinanceSplit.Domain.Entities;

public class MonthlyExpenseSummary : BaseEntity
{
    public MonthlyExpenseSummary(DateOnly month, IEnumerable<Transaction> transactions)
    {
        ArgumentNullException.ThrowIfNull(transactions);

        Month = new DateOnly(month.Year, month.Month, 1);

        var monthlyTransactions = transactions.ToArray();
        ValidateTransactionsMonth(monthlyTransactions, Month);
        _transactions = monthlyTransactions;
    }

    private readonly IReadOnlyList<Transaction> _transactions;

    public DateOnly Month { get; }
    public IReadOnlyCollection<Transaction> Transactions => _transactions;

    public IReadOnlyDictionary<Person, decimal> CalculateNetBalances()
    {
        var balances = new Dictionary<Person, decimal>(new PersonIdComparer());

        foreach (var transaction in _transactions)
        {
            var shares = transaction.SplitPay.CalculateShares(transaction.Amount, Month);

            AddToBalance(balances, transaction.PaidBy, transaction.Amount);

            foreach (var (person, share) in shares)
            {
                AddToBalance(balances, person, -share);
            }
        }

        return balances
            .Where(x => x.Value != 0)
            .ToDictionary(x => x.Key, x => decimal.Round(x.Value, 2, MidpointRounding.AwayFromZero), new PersonIdComparer());
    }

    public IReadOnlyCollection<ExpenseSettlement> CalculateSettlements()
    {
        var balances = CalculateNetBalances();
        var creditors = balances
            .Where(x => x.Value > 0)
            .Select(x => (Person: x.Key, Amount: x.Value))
            .OrderByDescending(x => x.Amount)
            .ThenBy(x => x.Person.Name, StringComparer.Ordinal)
            .ThenBy(x => x.Person.Id)
            .ToList();

        var debtors = balances
            .Where(x => x.Value < 0)
            .Select(x => (Person: x.Key, Amount: -x.Value))
            .OrderByDescending(x => x.Amount)
            .ThenBy(x => x.Person.Name, StringComparer.Ordinal)
            .ThenBy(x => x.Person.Id)
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
                settlements.Add(new ExpenseSettlement(debtor.Person, creditor.Person, settlementAmount));
            }

            creditor.Amount = decimal.Round(creditor.Amount - settlementAmount, 2, MidpointRounding.AwayFromZero);
            debtor.Amount = decimal.Round(debtor.Amount - settlementAmount, 2, MidpointRounding.AwayFromZero);

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

    private static void AddToBalance(Dictionary<Person, decimal> balances, Person person, decimal amount)
    {
        balances.TryGetValue(person, out var current);
        balances[person] = decimal.Round(current + amount, 2, MidpointRounding.AwayFromZero);
    }

    private static void ValidateTransactionsMonth(IEnumerable<Transaction> transactions, DateOnly month)
    {
        foreach (var transaction in transactions)
        {
            if (transaction.Recurrence is not null)
            {
                if (!transaction.Recurrence.OccursIn(month))
                {
                    throw new InvalidOperationException($"Recurring transaction '{transaction.Title}' does not occur in {month:yyyy-MM}.");
                }
            }
            else
            {
                var txDate = DateOnly.FromDateTime(transaction.Date);
                if (txDate.Year != month.Year || txDate.Month != month.Month)
                {
                    throw new InvalidOperationException("All one-off transactions must belong to the target month.");
                }
            }
        }
    }

    private sealed class PersonIdComparer : IEqualityComparer<Person>
    {
        public bool Equals(Person? x, Person? y) => x is null && y is null || x is not null && y is not null && x.Id == y.Id;

        public int GetHashCode(Person obj) => obj.Id.GetHashCode();
    }
}
