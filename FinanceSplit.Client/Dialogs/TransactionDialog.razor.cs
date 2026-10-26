using System.Globalization;
using FinanceSplit.Client.ViewModels;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FinanceSplit.Client.Dialogs;

public partial class TransactionDialog(TransactionModel model)
{
    private static readonly CultureInfo _enGb = CultureInfo.GetCultureInfo("en-GB");

    [Parameter]
    public TransactionResponse? Transaction { get; set; }

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    private TransactionModel Model { get; } = model;

    private static IReadOnlyList<SplitType> SplitTypes => Enum.GetValues<SplitType>();

    private string Title =>
        Model.IsEditing ? $"Edit transaction {Model.Name} {Model.Date}" : "Create transaction";

    private string SubmitLabel => Model.IsEditing ? "Edit" : "Create";

    private void Cancel() => MudDialog.Cancel();

    protected override async Task OnParametersSetAsync()
    {
        await Model.InitAsync(Transaction);
    }

    private async Task SaveTransaction()
    {
        var transactionSaved = await Model.SaveTransactionAsync();

        if (transactionSaved is not null)
        {
            MudDialog.Close(DialogResult.Ok(transactionSaved));
            return;
        }

        MudDialog.Cancel();
    }
}
