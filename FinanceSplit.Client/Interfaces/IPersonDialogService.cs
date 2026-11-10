using FinanceSplit.Contracts.DTOs;

namespace FinanceSplit.Client.Interfaces;

public interface IPersonDialogService
{
    Task<PersonResponse?> CreatePersonAsync();

    Task<PersonResponse?> EditPersonAsync(PersonResponse person);

    Task<bool> ShowDeletePersonConfirmationAsync(PersonResponse person);

    Task<IncomeRecordResponse?> CreateIncomeAsync(PersonResponse person);

    Task<IncomeRecordResponse?> EditIncomeAsync(
        PersonResponse person,
        IncomeRecordResponse incomeRecord
    );

    Task<bool> ShowDeleteIncomeConfirmationAsync(
        PersonResponse person,
        IncomeRecordResponse incomeRecord
    );
}
