using FinanceSplit.Contracts.Commands;
using FinanceSplit.Contracts.DTOs;

namespace FinanceSplit.Contracts.Services;

public interface IPersonService
{
    Task<IReadOnlyList<PersonDto>?> GetPeopleAsync(CancellationToken cancellationToken = default);

    Task<PersonDto?> CreatePersonAsync(
        CreatePersonCommand command,
        CancellationToken cancellationToken = default
    );

    Task<PersonDto?> UpdatePersonAsync(
        UpdatePersonCommand command,
        CancellationToken cancellationToken = default
    );

    Task DeletePersonAsync(
        DeletePersonCommand command,
        CancellationToken cancellationToken = default
    );

    Task<IncomeRecordResponse?> CreateIncomeRecordAsync(
        CreateIncomeRecordCommand command,
        CancellationToken cancellationToken = default
    );

    Task<IncomeRecordResponse?> UpdateIncomeRecordAsync(
        UpdateIncomeRecordCommand command,
        CancellationToken cancellationToken = default
    );

    Task DeleteIncomeRecordAsync(
        DeleteIncomeRecordCommand command,
        CancellationToken cancellationToken = default
    );
}
