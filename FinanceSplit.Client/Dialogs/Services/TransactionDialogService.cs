using FinanceSplit.Client.Extensions;
using FinanceSplit.Client.Interfaces;
using FinanceSplit.Contracts.Transactions.Commands;
using FinanceSplit.Contracts.Transactions.DTOs;
using MudBlazor;

namespace FinanceSplit.Client.Dialogs.Services;

public sealed class TransactionDialogService(IDialogService dialogService)
    : ITransactionDialogService
{
    private readonly DialogOptions _dialogOptions = new() { FullWidth = true };

    public Task<TransactionDto?> CreateTransactionAsync(
        CancellationToken cancellationToken = default
    ) => OpenDialogAsync();

    public Task<TransactionDto?> EditTransactionAsync(
        TransactionDto originalTransaction,
        CancellationToken cancellationToken = default
    ) => OpenDialogAsync(originalTransaction);

    public async Task<UpdateRecurrence?> GetRecurrenceUpdateTypeAsync(
        CancellationToken cancellationToken = default
    ) =>
        await dialogService.ShowDialogWithResultAsync<
            UpdateRecurrence,
            TransactionRecurrenceDialog
        >([], new());

    public async Task<bool> ShowDeleteTransactionConfirmationAsync(
        TransactionDto transaction,
        CancellationToken cancellationToken = default
    )
    {
        bool? result = await dialogService.ShowMessageBoxAsync(
            "Warning",
            "Deleting can not be undone!",
            yesText: "Delete!",
            cancelText: "Cancel"
        );

        return result == true;
    }

    private async Task<TransactionDto?> OpenDialogAsync(TransactionDto? transaction = null) =>
        await dialogService.ShowDialogWithResultAsync<TransactionDto, TransactionDialog>(
            new DialogParameters { ["Transaction"] = transaction },
            _dialogOptions
        );
}
