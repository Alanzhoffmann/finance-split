using FinanceSplit.Contracts.Enums;
using FinanceSplit.Contracts.Requests;
using FinanceSplit.Contracts.Responses;
using FinanceSplit.Web.Services;

namespace FinanceSplit.Web.ComponentTests;

public class MockApiClient : ApiClient
{
    private readonly List<PersonResponse> _people = [];
    private readonly List<TransactionResponse> _transactions = [];
    private readonly List<ExpenseSettlementResponse> _settlements = [];

    public MockApiClient()
        : base(new HttpClient()) { }

    public override Task<IReadOnlyList<PersonResponse>> GetPeopleAsync(CancellationToken ct = default)
    {
        return Task.FromResult<IReadOnlyList<PersonResponse>>(_people.AsReadOnly());
    }

    public override Task<PersonResponse?> CreatePersonAsync(string name, CancellationToken ct = default)
    {
        var person = new PersonResponse(Guid.NewGuid(), name, []);
        _people.Add(person);
        return Task.FromResult<PersonResponse?>(person);
    }

    public override Task<PersonResponse?> AddSalaryAsync(Guid personId, DateOnly date, decimal amount, CancellationToken ct = default)
    {
        var idx = _people.FindIndex(p => p.Id == personId);
        if (idx < 0)
            return Task.FromResult<PersonResponse?>(null);

        var existing = _people[idx];
        var salaries = existing.Salaries.Append(new SalaryResponse(date, amount)).ToList();
        var updated = new PersonResponse(existing.Id, existing.Name, salaries);
        _people[idx] = updated;
        return Task.FromResult<PersonResponse?>(updated);
    }

    public override Task<TransactionResponse?> CreateTransactionAsync(CreateTransactionRequest request, CancellationToken ct = default)
    {
        var paidBy = _people.FirstOrDefault(p => p.Id == request.PaidById);
        if (paidBy is null)
            return Task.FromResult<TransactionResponse?>(null);

        var participants = _people.Where(p => request.ParticipantIds.Contains(p.Id)).ToList();
        var tx = new TransactionResponse(
            Guid.NewGuid(),
            request.Title,
            string.Empty,
            request.Amount,
            request.Date ?? DateTime.UtcNow,
            paidBy,
            request.SplitType,
            participants,
            null
        );
        _transactions.Add(tx);
        return Task.FromResult<TransactionResponse?>(tx);
    }

    public override Task<TransactionResponse?> UpdateTransactionAsync(Guid id, CreateTransactionRequest request, CancellationToken ct = default)
    {
        var idx = _transactions.FindIndex(t => t.Id == id);
        if (idx < 0)
            return Task.FromResult<TransactionResponse?>(null);

        var paidBy = _people.FirstOrDefault(p => p.Id == request.PaidById);
        if (paidBy is null)
            return Task.FromResult<TransactionResponse?>(null);

        var participants = _people.Where(p => request.ParticipantIds.Contains(p.Id)).ToList();
        var updated = new TransactionResponse(
            id,
            request.Title,
            string.Empty,
            request.Amount,
            request.Date ?? _transactions[idx].Date,
            paidBy,
            request.SplitType,
            participants,
            null
        );
        _transactions[idx] = updated;
        return Task.FromResult<TransactionResponse?>(updated);
    }

    public override Task<MonthlyExpenseSummaryResponse?> GetMonthlySummaryAsync(DateOnly month, CancellationToken ct = default)
    {
        var monthTx = _transactions.Where(t => t.Date.Year == month.Year && t.Date.Month == month.Month).ToList();
        var netBalances = new Dictionary<PersonResponse, decimal>();
        foreach (var person in _people)
        {
            netBalances[person] = 0m;
        }
        var summary = new MonthlyExpenseSummaryResponse(Guid.Empty, month, monthTx, netBalances, _settlements);
        return Task.FromResult<MonthlyExpenseSummaryResponse?>(summary);
    }

    public override Task<IReadOnlyCollection<ExpenseSettlementResponse>> GetSettlementsAsync(DateOnly month, CancellationToken ct = default)
    {
        return Task.FromResult<IReadOnlyCollection<ExpenseSettlementResponse>>(_settlements);
    }

    public void AddSettlement(ExpenseSettlementResponse settlement)
    {
        _settlements.Add(settlement);
    }
}
