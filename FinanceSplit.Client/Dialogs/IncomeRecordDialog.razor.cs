using System.ComponentModel.DataAnnotations;
using FinanceSplit.Contracts.Commands;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Enums;
using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FinanceSplit.Client.Dialogs;

public partial class IncomeRecordDialog(IPersonService personService)
{
    [Parameter]
    [EditorRequired]
    public PersonResponse Person { get; set; }

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    private IncomeRecordModel Model
    {
        get => field ??= new(Person);
        set;
    }

    private bool IsEditing => Model.Id is not null;

    private string Title =>
        IsEditing ? $"Edit income {Model.Month} {Model.IncomeType}" : "Add income";

    private string SubmitLabel => IsEditing ? "Edit" : "Add";

    private void Cancel() => MudDialog.Cancel();

    protected override void OnParametersSet()
    {
        Model = new(Person);
    }

    private async Task SavePerson()
    {
        if (!IsEditing)
        {
            var newPerson = await personService.CreateIncomeRecordAsync(
                new CreateIncomeRecordCommand(
                    Model.PersonId,
                    Model.Amount,
                    Model.Month,
                    Model.IncomeType
                )
            );

            if (newPerson is not null)
            {
                MudDialog.Close(DialogResult.Ok(newPerson));
                return;
            }
        }
        else
        {
            //var updatedPerson = await personService.UpdatePersonAsync(
            //    new UpdatePersonCommand(Model.PersonId!.Value, Model.Amount!)
            //);

            //if (updatedPerson is not null)
            //{
            //    MudDialog.Close(DialogResult.Ok(updatedPerson));
            //    return;
            //}
        }

        MudDialog.Cancel();
    }

    class IncomeRecordModel
    {
        public IncomeRecordModel(PersonResponse person, IncomeRecordResponse? incomeRecord = null)
        {
            PersonId = person.Id;

            if (incomeRecord is not null)
            {
                Id = incomeRecord.Id;
                Amount = incomeRecord.Amount;
                Month = incomeRecord.Month;
                IncomeType = incomeRecord.Type;
            }
        }

        public Guid PersonId { get; set; }

        public Guid? Id { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateOnly Month { get; set; }

        public IncomeType IncomeType { get; set; } = IncomeType.Salary;
    }
}
