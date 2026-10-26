using System.ComponentModel.DataAnnotations;
using System.Globalization;
using FinanceSplit.Contracts.Commands;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Enums;
using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FinanceSplit.Client.Dialogs;

public partial class IncomeRecordDialog(IPersonService personService)
{
    private static readonly CultureInfo _enGb = CultureInfo.GetCultureInfo("en-GB");

    private static IReadOnlyList<IncomeType> IncomeTypes => Enum.GetValues<IncomeType>();

    [Parameter]
    [EditorRequired]
    public PersonResponse Person { get; set; }

    [Parameter]
    public IncomeRecordResponse? IncomeRecord { get; set; }

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    private IncomeRecordModel Model
    {
        get => field ??= new(Person, IncomeRecord);
        set;
    }

    private bool IsEditing => Model.Id is not null;

    private string Title =>
        IsEditing ? $"Edit income {Model.Month} {Model.IncomeType}" : "Add income";

    private string SubmitLabel => IsEditing ? "Edit" : "Add";

    private void Cancel() => MudDialog.Cancel();

    protected override void OnParametersSet()
    {
        Model = new(Person, IncomeRecord);
    }

    private async Task SaveIncome()
    {
        if (!IsEditing)
        {
            var newIncome = await personService.CreateIncomeRecordAsync(Model.CreateCommand);

            if (newIncome is not null)
            {
                MudDialog.Close(DialogResult.Ok(newIncome));
                return;
            }
        }
        else
        {
            var updatedIncome = await personService.UpdateIncomeRecordAsync(Model.UpdateCommand);

            if (updatedIncome is not null)
            {
                MudDialog.Close(DialogResult.Ok(updatedIncome));
                return;
            }
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
                Month = new DateTime(incomeRecord.Month, TimeOnly.MinValue);
                IncomeType = incomeRecord.Type;
            }
        }

        public Guid PersonId { get; set; }

        public Guid? Id { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime? Month { get; set; } = DateTime.Today;

        public IncomeType IncomeType { get; set; } = IncomeType.Salary;

        public CreateIncomeRecordCommand CreateCommand
        {
            get
            {
                if (Month is null)
                {
                    throw new ArgumentNullException(nameof(Month));
                }

                var month = DateOnly.FromDateTime(Month.Value);
                return new CreateIncomeRecordCommand(PersonId, Amount, month, IncomeType);
            }
        }

        public UpdateIncomeRecordCommand UpdateCommand
        {
            get
            {
                if (!Id.HasValue)
                {
                    throw new ArgumentNullException(nameof(Id));
                }

                if (Month is null)
                {
                    throw new ArgumentNullException(nameof(Month));
                }

                var month = DateOnly.FromDateTime(Month.Value);
                return new UpdateIncomeRecordCommand(Id.Value, PersonId, Amount, month, IncomeType);
            }
        }
    }
}
