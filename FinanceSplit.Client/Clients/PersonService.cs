using FinanceSplit.Contracts.Commands;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;

namespace FinanceSplit.Client.Clients;

public class PersonService(HttpClient client) : IPersonService
{
    public async Task<IReadOnlyList<PersonResponse>?> GetPeopleAsync(
        CancellationToken cancellationToken = default
    ) =>
        await client.GetFromJsonAsync<IReadOnlyList<PersonResponse>>(
            "api/person",
            cancellationToken: cancellationToken
        );

    public async Task<PersonResponse?> CreatePersonAsync(
        CreatePersonCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var response = await client.PostAsJsonAsync("api/person", command, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PersonResponse>(cancellationToken);
    }
}
