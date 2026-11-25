using FinanceSplit.Contracts.People.Commands;
using FinanceSplit.Contracts.People.DTOs;

namespace FinanceSplit.Contracts.People.Services;

public interface IPersonService
{
    Task<IReadOnlyList<PersonDto>?> GetPeopleAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PersonListDto>?> GetPeopleListAsync(
        CancellationToken cancellationToken = default
    );

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

    Task<IncomeRecordDto?> CreateIncomeRecordAsync(
        CreateIncomeRecordCommand command,
        CancellationToken cancellationToken = default
    );

    Task<IncomeRecordDto?> UpdateIncomeRecordAsync(
        UpdateIncomeRecordCommand command,
        CancellationToken cancellationToken = default
    );

    Task DeleteIncomeRecordAsync(
        DeleteIncomeRecordCommand command,
        CancellationToken cancellationToken = default
    );
}
