using System.Text.Json.Serialization;
using FinanceSplit.Contracts.Commands;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;

namespace FinanceSplit.Client.Clients;

internal partial class PersonService(HttpClient client) : IPersonService
{
    public async Task<IReadOnlyList<PersonResponse>?> GetPeopleAsync(
        CancellationToken cancellationToken = default
    ) =>
        await client.GetFromJsonAsync(
            "api/person",
            SerializerContext.Default.IReadOnlyListPersonResponse,
            cancellationToken: cancellationToken
        );

    public async Task<PersonResponse?> CreatePersonAsync(
        CreatePersonCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var response = await client.PostAsJsonAsync(
            "api/person",
            command,
            SerializerContext.Default.CreatePersonCommand,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync(
            SerializerContext.Default.PersonResponse,
            cancellationToken
        );
    }

    public async Task<PersonResponse?> UpdatePersonAsync(
        UpdatePersonCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var response = await client.PutAsJsonAsync(
            $"api/person/{command.Id}",
            command,
            SerializerContext.Default.UpdatePersonCommand,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync(
            SerializerContext.Default.PersonResponse,
            cancellationToken
        );
    }

    public async Task<IncomeRecordResponse?> CreateIncomeRecordAsync(
        CreateIncomeRecordCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var response = await client.PostAsJsonAsync(
            $"api/person/{command.PersonId}/income",
            command,
            SerializerContext.Default.CreateIncomeRecordCommand,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync(
            SerializerContext.Default.IncomeRecordResponse,
            cancellationToken
        );
    }

    public async Task<IncomeRecordResponse?> UpdateIncomeRecordAsync(
        UpdateIncomeRecordCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var response = await client.PutAsJsonAsync(
            $"api/person/{command.PersonId}/income/",
            command,
            SerializerContext.Default.UpdateIncomeRecordCommand,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync(
            SerializerContext.Default.IncomeRecordResponse,
            cancellationToken
        );
    }

    [JsonSerializable(typeof(IReadOnlyList<PersonResponse>))]
    [JsonSerializable(typeof(CreatePersonCommand))]
    [JsonSerializable(typeof(UpdatePersonCommand))]
    [JsonSerializable(typeof(CreateIncomeRecordCommand))]
    [JsonSerializable(typeof(UpdateIncomeRecordCommand))]
    partial class SerializerContext : JsonSerializerContext;
}
