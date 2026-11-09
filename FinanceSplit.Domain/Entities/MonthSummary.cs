using FinanceSplit.Contracts.Enums;
using FinanceSplit.Domain.ValueObjects;

namespace FinanceSplit.Domain.Entities;

public sealed class MonthSummary : BaseEntity
{
    private List<PersonSummary> _people = [];
    private List<ExpenseSettlement> _settlements = [];

    public required DateOnly Month { get; init; }
    public IList<ExpenseSettlement> Settlements
    {
        get => _settlements;
        set => _settlements = [.. value];
    }
    public IList<PersonSummary> People
    {
        get => _people;
        set => _people = [.. value];
    }

    public void Update(IEnumerable<Person> people)
    {
        var totalSharedPerPersonId = GetTotalSharedPerPerson(people);

        var settlements = CalculateSettlements(
            people
                .SelectMany(p => p.Transactions)
                .Where(t => t.Month == Month)
                .DistinctBy(t => t.Id)
        );

        _people =
        [
            .. people.Select(p => new PersonSummary
            {
                Person = p,
                Income = p.GetIncomeForMonth(Month),
                SharedExpenses = totalSharedPerPersonId.GetValueOrDefault(p.Id),
                PersonalExpenses = p
                    .Transactions.Where(t =>
                        t.SplitType is SplitType.None
                        && (t.SharedWith.Count == 0 || t.SharedWith[0].Id == t.PaidBy.Id)
                    )
                    .Sum(t => t.Value),
                TotalPaid = p.Transactions.Sum(t => t.Value),
            }),
        ];

        _settlements = [.. settlements];
    }

    private static IReadOnlyDictionary<Person, decimal> CalculateNetBalances(
        IEnumerable<Transaction> transactions
    )
    {
        var balances = new Dictionary<Person, decimal>();

        foreach (var transaction in transactions)
        {
            var shares = transaction.CalculateShares();

            AddToBalance(balances, transaction.PaidBy, transaction.Value);

            foreach (var (person, share) in shares)
            {
                AddToBalance(balances, person, -share);
            }
        }

        return balances
            .Where(x => x.Value != 0)
            .ToDictionary(
                x => x.Key,
                x => decimal.Round(x.Value, 2, MidpointRounding.AwayFromZero)
            );
    }

    private static IReadOnlyList<ExpenseSettlement> CalculateSettlements(
        IEnumerable<Transaction> transaction
    )
    {
        var balances = CalculateNetBalances(transaction);
        var creditors = balances
            .Where(x => x.Value > 0)
            .Select(x => (Person: x.Key, Amount: x.Value))
            .OrderByDescending(x => x.Amount)
            .ThenBy(x => x.Person)
            .ToList();

        var debtors = balances
            .Where(x => x.Value < 0)
            .Select(x => (Person: x.Key, Amount: -x.Value))
            .OrderByDescending(x => x.Amount)
            .ThenBy(x => x.Person)
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
                    new ExpenseSettlement(debtor.Person, creditor.Person, settlementAmount)
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
        Dictionary<Person, decimal> balances,
        Person person,
        decimal amount
    )
    {
        balances.TryGetValue(person, out var current);
        balances[person] = decimal.Round(current + amount, 2, MidpointRounding.AwayFromZero);
    }

    private static Dictionary<Guid, decimal> GetTotalSharedPerPerson(IEnumerable<Person> people) =>
        people
            .SelectMany(p => p.SharedTransactions)
            .DistinctBy(t => t.Id)
            .Where(t => t.SplitType is not SplitType.None)
            .SelectMany(t => t.CalculateShares())
            .GroupBy(g => g.Key.Id)
            .ToDictionary(g => g.Key, g => g.Sum(g1 => g1.Value));
}
