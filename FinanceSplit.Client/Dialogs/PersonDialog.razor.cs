using System.ComponentModel.DataAnnotations;
using FinanceSplit.Contracts.Commands;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FinanceSplit.Client.Dialogs;

public partial class PersonDialog(IPersonService personService)
{
    [Parameter]
    public PersonResponse? Person { get; set; }

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    private EditPersonModel Model
    {
        get => field ??= new();
        set;
    }

    private bool IsEditing => Model.Id is not null;

    private string Title => IsEditing ? $"Edit person {Model.Name}" : "Create person";

    private string SubmitLabel => IsEditing ? "Edit" : "Create";

    private void Cancel() => MudDialog.Cancel();

    protected override void OnParametersSet()
    {
        Model = new(Person);
    }

    private async Task SavePerson()
    {
        if (!IsEditing)
        {
            var newPerson = await personService.CreatePersonAsync(
                new CreatePersonCommand(Model.Name!)
            );

            if (newPerson is not null)
            {
                MudDialog.Close(DialogResult.Ok(newPerson));
                return;
            }
        }
        else
        {
            var updatedPerson = await personService.UpdatePersonAsync(
                new UpdatePersonCommand(Model.Id!.Value, Model.Name!)
            );

            if (updatedPerson is not null)
            {
                MudDialog.Close(DialogResult.Ok(updatedPerson));
                return;
            }
        }

        MudDialog.Cancel();
    }

    class EditPersonModel
    {
        public EditPersonModel(PersonResponse? person = null)
        {
            if (person is not null)
            {
                Id = person.Id;
                Name = person.Name;
            }
        }

        public Guid? Id { get; set; }

        [Required]
        public string? Name { get; set; }
    }
}
