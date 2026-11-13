using System.Text.Json.Serialization;
using FinanceSplit.Contracts.Services;
using FinanceSplit.Contracts.Transactions.Commands;
using FinanceSplit.Contracts.Transactions.Dtos;

namespace FinanceSplit.Client.Clients;

internal partial class TransactionService(HttpClient client) : ITransactionService
{
    public async Task<IReadOnlyList<TransactionDto>?> GetTransactionsAsync(
        DateOnly? month = null,
        CancellationToken cancellationToken = default
    )
    {
        var route = month is not null ? $"api/transaction?month={month}" : "api/transaction";
        return await client.GetFromJsonAsync(
            route,
            TransactionSerializerContext.Default.IReadOnlyListTransactionDto,
            cancellationToken: cancellationToken
        );
    }

    public async Task<TransactionDto?> CreateTransactionAsync(
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
            TransactionSerializerContext.Default.TransactionDto,
            cancellationToken
        );
    }

    public async Task<TransactionDto?> UpdateTransactionAsync(
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
            TransactionSerializerContext.Default.TransactionDto,
            cancellationToken
        );
    }

    public async Task DeleteTransactionAsync(
        DeleteTransactionCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var response = await client.DeleteAsync($"api/transaction/{command.Id}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<TransactionDto?> CreateRecurringTransactionAsync(
        CreateRecurringTransactionCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var response = await client.PostAsJsonAsync(
            "api/transaction/recurring",
            command,
            TransactionSerializerContext.Default.CreateRecurringTransactionCommand,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync(
            TransactionSerializerContext.Default.TransactionDto,
            cancellationToken
        );
    }

    public async Task<TransactionDto?> CreateSplitTransactionAsync(
        CreateSplitTransactionCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var response = await client.PostAsJsonAsync(
            "api/transaction/split",
            command,
            TransactionSerializerContext.Default.CreateSplitTransactionCommand,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync(
            TransactionSerializerContext.Default.TransactionDto,
            cancellationToken
        );
    }

    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(IReadOnlyList<TransactionDto>))]
    [JsonSerializable(typeof(CreateTransactionCommand))]
    [JsonSerializable(typeof(CreateRecurringTransactionCommand))]
    [JsonSerializable(typeof(CreateSplitTransactionCommand))]
    [JsonSerializable(typeof(UpdateTransactionCommand))]
    partial class TransactionSerializerContext : JsonSerializerContext;
}
