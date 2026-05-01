using FinanceSplit.Contracts.Requests;
using FinanceSplit.Contracts.Responses;
using FinanceSplit.Web.Services;

namespace FinanceSplit.Web.ComponentTests;

public class MockApiClient : ApiClient
{
    private readonly List<PersonResponse> _people = [];

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
        return Task.FromResult<TransactionResponse?>(null);
    }

    public override Task<MonthlyExpenseSummaryResponse?> GetMonthlySummaryAsync(DateOnly month, CancellationToken ct = default)
    {
        var summary = new MonthlyExpenseSummaryResponse(Guid.Empty, month, [], new Dictionary<PersonResponse, decimal>(), []);
        return Task.FromResult<MonthlyExpenseSummaryResponse?>(summary);
    }

    public override Task<IReadOnlyCollection<ExpenseSettlementResponse>> GetSettlementsAsync(DateOnly month, CancellationToken ct = default)
    {
        return Task.FromResult<IReadOnlyCollection<ExpenseSettlementResponse>>([]);
    }
}
