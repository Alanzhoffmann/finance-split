using FinanceSplit.Client.Extensions;
using FinanceSplit.Client.Interfaces;
using FinanceSplit.Contracts.Transactions.DTOs;
using MudBlazor;

namespace FinanceSplit.Client.Dialogs.Services;

public sealed class TransactionDialogService(IDialogService dialogService)
    : ITransactionDialogService
{
    private readonly DialogOptions _dialogOptions = new() { FullWidth = true };

    public Task<TransactionDto?> CreateTransactionAsync() => OpenDialogAsync();

    public Task<TransactionDto?> EditTransactionAsync(TransactionDto originalTransaction) =>
        OpenDialogAsync(originalTransaction);

    public async Task<bool> ShowDeleteTransactionConfirmationAsync(TransactionDto transaction) =>
        await dialogService.ShowGenericDeleteWarning();

    private async Task<TransactionDto?> OpenDialogAsync(TransactionDto? transaction = null) =>
        await dialogService.ShowDialogWithResultAsync<TransactionDto, TransactionDialog>(
            new DialogParameters { ["Transaction"] = transaction },
            _dialogOptions
        );
}
