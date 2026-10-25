using System.ComponentModel.DataAnnotations;
using FinanceSplit.Contracts.Commands;
using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FinanceSplit.Client.Pages;

public partial class EditPersonDialog(IPersonService personService)
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    private EditPersonModel Model { get; set; } = new();

    private Guid? Id { get; set; }

    private bool IsEditing => Id is not null;

    private void Cancel() => MudDialog.Cancel();

    private async Task SavePerson()
    {
        if (!IsEditing)
        {
            var newPerson = await personService.CreatePersonAsync(
                new CreatePersonCommand(Model.Name!)
            );
            MudDialog.Close(DialogResult.Ok(newPerson));
        }
    }

    class EditPersonModel
    {
        [Required]
        public string? Name { get; set; }
    }
}
