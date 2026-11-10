using FinanceSplit.Client.Extensions;
using FinanceSplit.Client.Interfaces;
using FinanceSplit.Contracts.DTOs;
using MudBlazor;

namespace FinanceSplit.Client.Dialogs.Services;

public sealed class PersonDialogService(IDialogService dialogService) : IPersonDialogService
{
    private readonly DialogOptions _dialogOptions = new() { FullWidth = true };

    public Task<PersonResponse?> CreatePersonAsync() => OpenPersonDialogAsync();

    public Task<PersonResponse?> EditPersonAsync(PersonResponse person) =>
        OpenPersonDialogAsync(person);

    public async Task<bool> ShowDeletePersonConfirmationAsync(PersonResponse person)
    {
        bool? result = await dialogService.ShowMessageBoxAsync(
            "Warning",
            "Deleting can not be undone!",
            yesText: "Delete!",
            cancelText: "Cancel"
        );

        return result == true;
    }

    public Task<IncomeRecordResponse?> CreateIncomeAsync(PersonResponse person) =>
        OpenIncomeDialogAsync(person);

    public Task<IncomeRecordResponse?> EditIncomeAsync(
        PersonResponse person,
        IncomeRecordResponse incomeRecord
    ) => OpenIncomeDialogAsync(person, incomeRecord);

    public async Task<bool> ShowDeleteIncomeConfirmationAsync(
        PersonResponse person,
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

    private async Task<PersonResponse?> OpenPersonDialogAsync(PersonResponse? person = null) =>
        await dialogService.ShowDialogAsync<PersonResponse, PersonDialog>(
            new DialogParameters { ["Person"] = person },
            _dialogOptions
        );

    private async Task<IncomeRecordResponse?> OpenIncomeDialogAsync(
        PersonResponse person,
        IncomeRecordResponse? incomeRecord = null
    ) =>
        await dialogService.ShowDialogAsync<IncomeRecordResponse, IncomeRecordDialog>(
            new DialogParameters { ["Person"] = person, ["IncomeRecord"] = incomeRecord },
            _dialogOptions
        );
}
