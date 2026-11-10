using FinanceSplit.Client.Extensions;
using FinanceSplit.Client.Interfaces;
using FinanceSplit.Contracts.DTOs;
using MudBlazor;

namespace FinanceSplit.Client.Dialogs.Services;

public sealed class TransactionDialogService(IDialogService dialogService)
    : ITransactionDialogService
{
    private readonly DialogOptions _dialogOptions = new() { FullWidth = true };

    public Task<TransactionResponse?> CreateTransactionAsync() => OpenDialogAsync();

    public Task<TransactionResponse?> EditTransactionAsync(
        TransactionResponse originalTransaction
    ) => OpenDialogAsync(originalTransaction);

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

    private async Task<TransactionResponse?> OpenDialogAsync(
        TransactionResponse? transaction = null
    ) =>
        await dialogService.ShowDialogAsync<TransactionResponse, TransactionDialog>(
            new DialogParameters { ["Transaction"] = transaction },
            _dialogOptions
        );
}
