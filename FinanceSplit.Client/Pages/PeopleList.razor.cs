using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FinanceSplit.Client.Pages;

public partial class PeopleList(IPersonService personService, IDialogService dialogService)
{
    [PersistentState(AllowUpdates = true)]
    public IReadOnlyList<PersonResponse>? People { get; set; }

    protected override async Task OnInitializedAsync()
    {
        People ??= await personService.GetPeopleAsync();
    }

    private async Task OpenCreatePersonDialog()
    {
        var dialog = await dialogService.ShowAsync<EditPersonDialog>();
        var result = await dialog.Result;

        if (!result?.Canceled ?? false)
        {
            People = await personService.GetPeopleAsync();
        }
    }
}
