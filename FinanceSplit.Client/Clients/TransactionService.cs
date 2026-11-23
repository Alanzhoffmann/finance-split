using System.Text.Json.Serialization;
using FinanceSplit.Contracts.Transactions.Commands;
using FinanceSplit.Contracts.Transactions.DTOs;
using FinanceSplit.Contracts.Transactions.Services;
using Microsoft.AspNetCore.WebUtilities;

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

    public async Task<TransactionDto?> UpdateRecurringTransactionAsync(
        UpdateRecurringTransactionCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var parameters = new Dictionary<string, string?>();
        if (command.TransactionId.HasValue)
        {
            parameters.Add("TransactionId", command.TransactionId.ToString());
        }

        var response = await client.PutAsJsonAsync(
            QueryHelpers.AddQueryString(
                $"api/transaction/recurring/{command.RecurrenceId}",
                parameters
            ),
            command,
            TransactionSerializerContext.Default.UpdateRecurringTransactionCommand,
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
    [JsonSerializable(typeof(UpdateTransactionCommand))]
    [JsonSerializable(typeof(UpdateRecurringTransactionCommand))]
    partial class TransactionSerializerContext : JsonSerializerContext;
}
