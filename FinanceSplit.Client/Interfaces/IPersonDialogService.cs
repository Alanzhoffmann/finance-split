using FinanceSplit.Contracts.DTOs;

namespace FinanceSplit.Client.Interfaces;

public interface IPersonDialogService
{
    Task<PersonDto?> CreatePersonAsync();

    Task<PersonDto?> EditPersonAsync(PersonDto person);

    Task<bool> ShowDeletePersonConfirmationAsync(PersonDto person);

    Task<IncomeRecordResponse?> CreateIncomeAsync(PersonDto person);

    Task<IncomeRecordResponse?> EditIncomeAsync(
        PersonDto person,
        IncomeRecordResponse incomeRecord
    );

    Task<bool> ShowDeleteIncomeConfirmationAsync(
        PersonDto person,
        IncomeRecordResponse incomeRecord
    );
}
