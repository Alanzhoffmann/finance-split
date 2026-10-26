using System.Text.Json.Serialization;
using FinanceSplit.Contracts.Commands;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;

namespace FinanceSplit.Client.Clients;

internal partial class TransactionService(HttpClient client) : ITransactionService
{
    public async Task<IReadOnlyList<TransactionResponse>?> GetTransactionsAsync(
        DateTime? month = null,
        CancellationToken cancellationToken = default
    )
    {
        var route = month is not null ? $"api/transaction?month={month}" : "api/transaction";
        return await client.GetFromJsonAsync(
            route,
            TransactionSerializerContext.Default.IReadOnlyListTransactionResponse,
            cancellationToken: cancellationToken
        );
    }

    public async Task<TransactionResponse?> CreateTransactionAsync(
        CreateTransactionCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var response = await client.PostAsJsonAsync(
            "api/transaction",
            command,
            TransactionSerializerContext.Default.CreateTransactionCommand,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync(
            TransactionSerializerContext.Default.TransactionResponse,
            cancellationToken
        );
    }

    public async Task<TransactionResponse?> UpdateTransactionAsync(
        UpdateTransactionCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var response = await client.PutAsJsonAsync(
            $"api/transaction/{command.Id}",
            command,
            TransactionSerializerContext.Default.UpdateTransactionCommand,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync(
            TransactionSerializerContext.Default.TransactionResponse,
            cancellationToken
        );
    }

    [JsonSerializable(typeof(IReadOnlyList<TransactionResponse>))]
    [JsonSerializable(typeof(CreateTransactionCommand))]
    [JsonSerializable(typeof(UpdateTransactionCommand))]
    partial class TransactionSerializerContext : JsonSerializerContext;
}
