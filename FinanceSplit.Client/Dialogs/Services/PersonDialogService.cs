using FinanceSplit.Client.Extensions;
using FinanceSplit.Client.Interfaces;
using FinanceSplit.Contracts.DTOs;
using MudBlazor;

namespace FinanceSplit.Client.Dialogs.Services;

public sealed class PersonDialogService(IDialogService dialogService) : IPersonDialogService
{
    private readonly DialogOptions _dialogOptions = new() { FullWidth = true };

    public Task<PersonDto?> CreatePersonAsync() => OpenPersonDialogAsync();

    public Task<PersonDto?> EditPersonAsync(PersonDto person) => OpenPersonDialogAsync(person);

    public async Task<bool> ShowDeletePersonConfirmationAsync(PersonDto person)
    {
        bool? result = await dialogService.ShowMessageBoxAsync(
            "Warning",
            "Deleting can not be undone!",
            yesText: "Delete!",
            cancelText: "Cancel"
        );

        return result == true;
    }

    public Task<IncomeRecordResponse?> CreateIncomeAsync(PersonDto person) =>
        OpenIncomeDialogAsync(person);

    public Task<IncomeRecordResponse?> EditIncomeAsync(
        PersonDto person,
        IncomeRecordResponse incomeRecord
    ) => OpenIncomeDialogAsync(person, incomeRecord);

    public async Task<bool> ShowDeleteIncomeConfirmationAsync(
        PersonDto person,
        IncomeRecordResponse incomeRecord
    )
    {
        bool? result = await dialogService.ShowMessageBoxAsync(
            "Warning",
            "Deleting can not be undone!",
            yesText: "Delete!",
            cancelText: "Cancel"
        );

        return result == true;
    }

    private async Task<PersonDto?> OpenPersonDialogAsync(PersonDto? person = null) =>
        await dialogService.ShowDialogWithResultAsync<PersonDto, PersonDialog>(
            new DialogParameters { ["Person"] = person },
            _dialogOptions
        );

    private async Task<IncomeRecordResponse?> OpenIncomeDialogAsync(
        PersonDto person,
        IncomeRecordResponse? incomeRecord = null
    ) =>
        await dialogService.ShowDialogWithResultAsync<IncomeRecordResponse, IncomeRecordDialog>(
            new DialogParameters { ["Person"] = person, ["IncomeRecord"] = incomeRecord },
            _dialogOptions
        );
}
