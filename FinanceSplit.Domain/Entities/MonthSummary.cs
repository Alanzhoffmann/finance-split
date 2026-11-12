using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Enums;
using FinanceSplit.Domain.ValueObjects;

namespace FinanceSplit.Domain.Entities;

public sealed class MonthSummary
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

    public void Update(IEnumerable<Person> people, IEnumerable<TransactionDto> transactions)
    {
        var personById = people.ToDictionary(p => p.Id);

        _people = CalculatePeopleSummary(people, transactions);

        _settlements =
        [
            .. CalculateSettlements(transactions)
                .Select(e => new ExpenseSettlement(
                    personById[e.From.Id],
                    personById[e.To.Id],
                    e.Amount
                )),
        ];
    }

    private static Dictionary<Guid, decimal> CalculateNetBalances(
        IEnumerable<TransactionDto> transactions
    )
    {
        var balances = new Dictionary<Guid, decimal>();

        foreach (var transaction in transactions)
        {
            var shares = transaction.SharedWith;

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

    private static void AddToBalance(
        Dictionary<Guid, decimal> balances,
        Guid person,
        decimal amount
    )
    {
        balances.TryGetValue(person, out var current);
        balances[person] = decimal.Round(current + amount, 2, MidpointRounding.AwayFromZero);
    }

    private List<PersonSummary> CalculatePeopleSummary(
        IEnumerable<Person> people,
        IEnumerable<TransactionDto> transactions
    )
    {
        var totalSharedPerPersonId = GetTotalSharedPerPerson(transactions);

        return
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
    }

    private List<ExpenseSettlementDto> CalculateSettlements(
        IEnumerable<TransactionDto> transactions
    )
    {
        var balances = CalculateNetBalances(
            transactions.Where(t => t.Month == Month).DistinctBy(t => t.Id)
        );

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

        var settlements = new List<ExpenseSettlementDto>();
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
                    new ExpenseSettlementDto(
                        new(debtor.Person, string.Empty),
                        new(creditor.Person, string.Empty),
                        settlementAmount
                    )
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

    private Dictionary<Guid, decimal> GetTotalSharedPerPerson(
        IEnumerable<TransactionDto> transactions
    ) =>
        transactions
            .Where(t => t.Month == Month && t.SplitType is not SplitType.None)
            .DistinctBy(t => t.Id)
            .SelectMany(t => t.SharedWith)
            .GroupBy(g => g.Person.Id)
            .ToDictionary(g => g.Key, g => g.Sum(g1 => g1.Amount));
}
