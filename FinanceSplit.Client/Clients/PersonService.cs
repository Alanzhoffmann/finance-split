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
}
