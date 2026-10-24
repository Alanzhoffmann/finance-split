using FinanceSplit.Contracts.DTOs;

namespace FinanceSplit.Contracts.Services;

public interface IPersonService
{
    Task<IReadOnlyList<PersonResponse>?> GetPeopleAsync(
        CancellationToken cancellationToken = default
    );
}
