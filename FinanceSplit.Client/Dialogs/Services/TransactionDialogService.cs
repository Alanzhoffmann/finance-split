using FinanceSplit.Client.Interfaces;
using FinanceSplit.Contracts.DTOs;
using MudBlazor;

namespace FinanceSplit.Client.Dialogs.Services;

public sealed class TransactionDialogService(IDialogService dialogService)
    : ITransactionDialogService
{
    private readonly DialogOptions _dialogOptions = new() { FullWidth = true };

    public Task<TransactionResponse?> CreateTransactionAsync() => OpenDialog();

    public Task<TransactionResponse?> EditTransactionAsync(
        TransactionResponse originalTransaction
    ) => OpenDialog(originalTransaction);

    public async Task<bool> ShowDeleteTransactionConfirmationAsync(TransactionResponse transaction)
    {
        bool? result = await dialogService.ShowMessageBoxAsync(
            "Warning",
            "Deleting can not be undone!",
            yesText: "Delete!",
            cancelText: "Cancel"
        );

        return result == true;
    }

    private async Task<TransactionResponse?> OpenDialog(TransactionResponse? transaction = null)
    {
        var parameters = new DialogParameters { ["Transaction"] = transaction };
        var dialog = await dialogService.ShowAsync<TransactionDialog>(parameters, _dialogOptions);
        var result = await dialog.Result;

        return
            result is not null
            && !result.Canceled
            && result.Data is TransactionResponse editedTransaction
            ? editedTransaction
            : null;
    }
}
