using FinanceSplit.Contracts.People.DTOs;

namespace FinanceSplit.Client.Interfaces;

public interface IPersonDialogService
{
    Task<PersonDto?> CreatePersonAsync();

    Task<PersonDto?> EditPersonAsync(PersonDto person);

    Task<bool> ShowDeletePersonConfirmationAsync(PersonDto person);

    Task<IncomeRecordDto?> CreateIncomeAsync(PersonDto person);

    Task<IncomeRecordDto?> EditIncomeAsync(PersonDto person, IncomeRecordDto incomeRecord);

    Task<bool> ShowDeleteIncomeConfirmationAsync(PersonDto person, IncomeRecordDto incomeRecord);
}
