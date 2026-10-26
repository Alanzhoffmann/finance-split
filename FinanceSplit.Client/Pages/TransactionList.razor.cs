using FinanceSplit.Client.Dialogs;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FinanceSplit.Client.Pages;

public partial class TransactionList(
    ITransactionService transactionService,
    IDialogService dialogService
)
{
    private readonly DialogOptions _dialogOptions = new() { FullWidth = true };

    [PersistentState(AllowUpdates = true)]
    public IReadOnlyList<TransactionResponse>? Transactions { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Transactions ??= await transactionService.GetTransactionsAsync();
    }

    private async Task OpenCreateTransactionDialog()
    {
        var dialog = await dialogService.ShowAsync<TransactionDialog>(_dialogOptions);
        var result = await dialog.Result;

        if (!result?.Canceled ?? false)
        {
            Transactions = await transactionService.GetTransactionsAsync();
        }
    }

    private async Task OpenEditTransactionDialog(TransactionResponse transaction)
    {
        var dialog = await dialogService.ShowAsync<TransactionDialog>(
            new DialogParameters { ["Transaction"] = transaction },
            _dialogOptions
        );
        var result = await dialog.Result;

        if (!result?.Canceled ?? false)
        {
            Transactions = await transactionService.GetTransactionsAsync();
        }
    }
}
