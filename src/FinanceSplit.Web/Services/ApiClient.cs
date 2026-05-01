using System.Net.Http.Json;
using FinanceSplit.Contracts.Requests;
using FinanceSplit.Contracts.Responses;

namespace FinanceSplit.Web.Services;

public class ApiClient(HttpClient http)
{
    public virtual async Task<IReadOnlyList<PersonResponse>> GetPeopleAsync(CancellationToken ct = default)
    {
        var result = await http.GetFromJsonAsync<List<PersonResponse>>("/api/people", ct);
        return result ?? [];
    }

    public virtual async Task<PersonResponse?> CreatePersonAsync(string name, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("/api/people", new CreatePersonRequest(name), ct);
        if (!response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadFromJsonAsync<PersonResponse>(ct);
    }

    public virtual async Task<PersonResponse?> UpdatePersonNameAsync(Guid personId, string name, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"/api/people/{personId}/name", new UpdatePersonNameRequest(name), ct);
        if (!response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadFromJsonAsync<PersonResponse>(ct);
    }

    public virtual async Task<PersonResponse?> AddSalaryAsync(Guid personId, DateOnly date, decimal amount, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync($"/api/people/{personId}/salaries", new AddSalaryRequest(date, amount), ct);
        if (!response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadFromJsonAsync<PersonResponse>(ct);
    }

    public virtual async Task<TransactionResponse?> CreateTransactionAsync(CreateTransactionRequest request, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("/api/transactions", request, ct);
        if (!response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadFromJsonAsync<TransactionResponse>(ct);
    }

    public virtual async Task<TransactionResponse?> UpdateTransactionAsync(Guid id, CreateTransactionRequest request, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"/api/transactions/{id}", request, ct);
        if (!response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadFromJsonAsync<TransactionResponse>(ct);
    }

    public virtual async Task<MonthlyExpenseSummaryResponse?> GetMonthlySummaryAsync(DateOnly month, CancellationToken ct = default)
    {
        return await http.GetFromJsonAsync<MonthlyExpenseSummaryResponse>($"/api/expenses/summary/{month.Year}/{month.Month}", ct);
    }

    public virtual async Task<IReadOnlyCollection<ExpenseSettlementResponse>> GetSettlementsAsync(DateOnly month, CancellationToken ct = default)
    {
        var result = await http.GetFromJsonAsync<List<ExpenseSettlementResponse>>($"/api/expenses/settlements/{month.Year}/{month.Month}", ct);
        return result ?? [];
    }

    public virtual async Task<ImportBackupResponse?> ImportBackupAsync(ImportBackupRequest request, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("/api/import/backup", request, ct);
        if (!response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadFromJsonAsync<ImportBackupResponse>(ct);
    }
}
