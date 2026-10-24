using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;

namespace FinanceSplit.Client.Clients;

public class TransactionService(HttpClient client) : ITransactionService
{
    public async Task<IReadOnlyList<TransactionResponse>?> GetTransactionsAsync(
        DateTime? month = null,
        CancellationToken cancellationToken = default
    )
    {
        var route = month is not null ? $"api/transaction?month={month}" : "api/transaction";
        return await client.GetFromJsonAsync<IReadOnlyList<TransactionResponse>>(
            route,
            cancellationToken: cancellationToken
        );
    }
}
