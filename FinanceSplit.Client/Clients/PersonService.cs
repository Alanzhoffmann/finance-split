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
            PersonSerializerContext.Default.IReadOnlyListPersonResponse,
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
            PersonSerializerContext.Default.CreatePersonCommand,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync(
            PersonSerializerContext.Default.PersonResponse,
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
            PersonSerializerContext.Default.UpdatePersonCommand,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync(
            PersonSerializerContext.Default.PersonResponse,
            cancellationToken
        );
    }

    public async Task DeletePersonAsync(
        DeletePersonCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var response = await client.DeleteAsync($"api/person/{command.Id}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<IncomeRecordResponse?> CreateIncomeRecordAsync(
        CreateIncomeRecordCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var response = await client.PostAsJsonAsync(
            $"api/person/{command.PersonId}/income",
            command,
            PersonSerializerContext.Default.CreateIncomeRecordCommand,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync(
            PersonSerializerContext.Default.IncomeRecordResponse,
            cancellationToken
        );
    }

    public async Task<IncomeRecordResponse?> UpdateIncomeRecordAsync(
        UpdateIncomeRecordCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var response = await client.PutAsJsonAsync(
            $"api/person/{command.PersonId}/income/{command.Id}",
            command,
            PersonSerializerContext.Default.UpdateIncomeRecordCommand,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync(
            PersonSerializerContext.Default.IncomeRecordResponse,
            cancellationToken
        );
    }

    public async Task DeleteIncomeRecordAsync(
        DeleteIncomeRecordCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var response = await client.DeleteAsync(
            $"api/person/{command.PersonId}/income/{command.Id}",
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
    }

    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(IReadOnlyList<PersonResponse>))]
    [JsonSerializable(typeof(CreatePersonCommand))]
    [JsonSerializable(typeof(UpdatePersonCommand))]
    [JsonSerializable(typeof(CreateIncomeRecordCommand))]
    [JsonSerializable(typeof(UpdateIncomeRecordCommand))]
    partial class PersonSerializerContext : JsonSerializerContext;
}
