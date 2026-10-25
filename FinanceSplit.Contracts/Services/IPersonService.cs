using FinanceSplit.Contracts.Commands;
using FinanceSplit.Contracts.DTOs;

namespace FinanceSplit.Contracts.Services;

public interface IPersonService
{
    Task<IReadOnlyList<PersonResponse>?> GetPeopleAsync(
        CancellationToken cancellationToken = default
    );

    Task<PersonResponse?> CreatePersonAsync(
        CreatePersonCommand command,
        CancellationToken cancellationToken = default
    );

    Task<PersonResponse?> UpdatePersonAsync(
        UpdatePersonCommand command,
        CancellationToken cancellationToken = default
    );
}
