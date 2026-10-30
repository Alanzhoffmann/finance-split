using System.Globalization;
using FinanceSplit.Client.ViewModels;
using FinanceSplit.Contracts.DTOs;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FinanceSplit.Client.Dialogs;

public partial class TransactionDialog(TransactionViewModel viewModel)
{
    private static readonly CultureInfo _enGb = CultureInfo.GetCultureInfo("en-GB");

    [Parameter]
    public TransactionResponse? Transaction { get; set; }

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    private TransactionViewModel ViewModel { get; } = viewModel;

    private string Title =>
        ViewModel.IsEditing
            ? $"Edit transaction {ViewModel.Name} {ViewModel.Date}"
            : "Create transaction";

    private string SubmitLabel => ViewModel.IsEditing ? "Edit" : "Create";

    private void Cancel() => MudDialog.Cancel();

    protected override async Task OnParametersSetAsync() => await ViewModel.InitAsync(Transaction);

    private async Task SaveTransaction()
    {
        var transactionSaved = await ViewModel.SaveTransactionAsync();

        if (transactionSaved is not null)
        {
            MudDialog.Close(DialogResult.Ok(transactionSaved));
            return;
        }

        MudDialog.Cancel();
    }
}
