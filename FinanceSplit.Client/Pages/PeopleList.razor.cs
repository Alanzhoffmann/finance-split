using FinanceSplit.Client.Dialogs;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FinanceSplit.Client.Pages;

public partial class PeopleList(IPersonService personService, IDialogService dialogService)
{
    private readonly DialogOptions _dialogOptions = new() { FullWidth = true };
    private readonly TableGroupDefinition<IncomeRecordResponse> _groupDefinition = new()
    {
        GroupName = "Month",
        Indentation = true,
        Expandable = false,
        Selector = (e) => e.Month.ToString("MMM/yyyy"),
    };

    [PersistentState(AllowUpdates = true)]
    public IReadOnlyList<PersonResponse>? People { get; set; }

    protected override async Task OnInitializedAsync()
    {
        People ??= await personService.GetPeopleAsync();
    }

    private async Task OpenCreatePersonDialog()
    {
        var dialog = await dialogService.ShowAsync<PersonDialog>(_dialogOptions);
        var result = await dialog.Result;

        if (!result?.Canceled ?? false)
        {
            People = await personService.GetPeopleAsync();
        }
    }

    private async Task OpenEditPersonDialog(PersonResponse person)
    {
        var dialog = await dialogService.ShowAsync<PersonDialog>(
            new DialogParameters { ["Person"] = person },
            _dialogOptions
        );

        var result = await dialog.Result;

        if (!result?.Canceled ?? false)
        {
            People = await personService.GetPeopleAsync();
        }
    }

    private async Task OpenAddIncomeDialog(PersonResponse person)
    {
        var dialog = await dialogService.ShowAsync<IncomeRecordDialog>(
            new DialogParameters { ["Person"] = person },
            _dialogOptions
        );
        var result = await dialog.Result;

        if (!result?.Canceled ?? false)
        {
            People = await personService.GetPeopleAsync();
        }
    }

    private async Task OpenEditIncomeDialog(
        PersonResponse person,
        IncomeRecordResponse incomeRecord
    )
    {
        var dialog = await dialogService.ShowAsync<IncomeRecordDialog>(
            new DialogParameters { ["Person"] = person, ["IncomeRecord"] = incomeRecord },
            _dialogOptions
        );
        var result = await dialog.Result;

        if (!result?.Canceled ?? false)
        {
            People = await personService.GetPeopleAsync();
        }
    }
}
