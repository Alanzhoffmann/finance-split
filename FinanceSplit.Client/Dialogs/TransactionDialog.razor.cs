using System.ComponentModel.DataAnnotations;
using System.Globalization;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Enums;
using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FinanceSplit.Client.Dialogs;

public partial class TransactionDialog(
    ITransactionService transactionService,
    IPersonService personService
)
{
    private static readonly CultureInfo _enGb = CultureInfo.GetCultureInfo("en-GB");

    [Parameter]
    public TransactionResponse? Transaction { get; set; }

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    private TransactionModel Model
    {
        get => field ??= new();
        set;
    }

    private IReadOnlyList<PersonResponse> People { get; set; } = [];

    private static IReadOnlyList<SplitType> SplitTypes => Enum.GetValues<SplitType>();

    private bool IsEditing => Model.Id is not null;

    private string Title =>
        IsEditing ? $"Edit transaction {Model.Name} {Model.Date}" : "Create transaction";

    private string SubmitLabel => IsEditing ? "Edit" : "Create";

    private void Cancel() => MudDialog.Cancel();

    protected override async Task OnParametersSetAsync()
    {
        Model = new(Transaction);
        People = await personService.GetPeopleAsync() ?? [];
        if (Model.PaidByPersonId == default && People.Count > 0)
        {
            Model.PaidByPersonId = People[0].Id;
        }
    }

    private async Task SaveTransaction()
    {
        if (!IsEditing)
        {
            var newTransaction = await transactionService.CreateTransactionAsync(
                Model.CreateCommand
            );

            if (newTransaction is not null)
            {
                MudDialog.Close(DialogResult.Ok(newTransaction));
                return;
            }
        }
        else
        {
            //var updatedPerson = await service.UpdatePersonAsync(
            //    new UpdatePersonCommand(Model.Id!.Value, Model.Name!)
            //);

            //if (updatedPerson is not null)
            //{
            //    MudDialog.Close(DialogResult.Ok(updatedPerson));
            //    return;
            //}
        }

        MudDialog.Cancel();
    }

    private string GetMultiSelectionText(IReadOnlyList<string> selectedValues)
    {
        return string.Join(
            ", ",
            People.Where(p => selectedValues.Contains(p.Id.ToString())).Select(p => p.Name).Order()
        );
    }

    class TransactionModel
    {
        public TransactionModel(TransactionResponse? transaction = null)
        {
            if (transaction is not null)
            {
                Id = transaction.Id;
                Name = transaction.Name;
            }
        }

        public Guid? Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public DateTime? Date { get; set; }
        public decimal Value { get; set; }
        public Guid PaidByPersonId { get; set; }
        public bool IsRecurring { get; set; }
        public SplitType SplitType { get; set; }
        public IReadOnlyCollection<Guid> SharedWithIds { get; set; } = [];

        public CreateTransactionCommand CreateCommand
        {
            get
            {
                var name = Name ?? throw new NullReferenceException("Name is null");
                var date = Date is null
                    ? throw new NullReferenceException("Date is null")
                    : DateOnly.FromDateTime(Date.Value);

                Guid[] sharedWith = [];
                if (SplitType is not SplitType.None)
                {
                    sharedWith = [.. SharedWithIds];
                }

                return new CreateTransactionCommand(
                    name,
                    date,
                    Value,
                    PaidByPersonId,
                    IsRecurring,
                    SplitType,
                    sharedWith
                );
            }
        }
    }
}
