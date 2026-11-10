using FinanceSplit.Client.Interfaces;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FinanceSplit.Client.Pages;

public partial class PersonCard(IPersonService personService, IPersonDialogService dialogService)
{
    private readonly TableGroupDefinition<IncomeRecordResponse> _groupDefinition = new()
    {
        GroupName = "Month",
        Indentation = true,
        Expandable = false,
        Selector = (e) => e.Month.ToString("MMM/yyyy"),
    };

    [Parameter]
    [EditorRequired]
    public PersonResponse Person { get; set; }

    private async Task OpenEditPersonDialog(PersonResponse person)
    {
        var editedPerson = await dialogService.EditPersonAsync(person);
        if (editedPerson is not null)
        {
            Person = editedPerson;
        }
    }

    private async Task DeletePerson(PersonResponse person)
    {
        var result = await dialogService.ShowDeletePersonConfirmationAsync(person);
        if (result == true)
        {
            await personService.DeletePersonAsync(new(person.Id));
            //People = await personService.GetPeopleAsync();
        }
    }

    private async Task OpenAddIncomeDialog(PersonResponse person)
    {
        var newIncome = await dialogService.CreateIncomeAsync(person);
        if (newIncome is not null)
        {
            // TODO refresh income list
        }
    }

    private async Task OpenEditIncomeDialog(
        PersonResponse person,
        IncomeRecordResponse incomeRecord
    )
    {
        var editedIncome = await dialogService.EditIncomeAsync(person, incomeRecord);
        if (editedIncome is not null)
        {
            //People = await personService.GetPeopleAsync();
        }
    }

    private async Task DeleteIncome(PersonResponse person, IncomeRecordResponse incomeRecord)
    {
        var result = await dialogService.ShowDeleteIncomeConfirmationAsync(person, incomeRecord);
        if (result == true)
        {
            await personService.DeleteIncomeRecordAsync(new(incomeRecord.Id, person.Id));
            //People = await personService.GetPeopleAsync();
        }
    }
}
