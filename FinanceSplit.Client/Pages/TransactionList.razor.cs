using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using FinanceSplit.Client.Dialogs;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FinanceSplit.Client.Pages;

[INotifyPropertyChanged]
public partial class TransactionList(
    ITransactionService transactionService,
    IDialogService dialogService,
    TimeProvider timeProvider
)
{
    private static readonly CultureInfo _enGb = CultureInfo.GetCultureInfo("en-GB");
    private readonly DialogOptions _dialogOptions = new() { FullWidth = true };
    private MudMessageBox _deleteConfirmationMessageBox = default!;

    [ObservableProperty]
    private partial DateTime? Month { get; set; } = timeProvider.GetLocalNow().Date;

    [PersistentState(AllowUpdates = true)]
    public IReadOnlyList<TransactionResponse>? Transactions { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        DateOnly? month = Month.HasValue ? DateOnly.FromDateTime(Month.Value) : null;
        Transactions ??= await transactionService.GetTransactionsAsync(month);
    }

    async partial void OnMonthChanged(DateTime? value)
    {
        DateOnly? month = value.HasValue ? DateOnly.FromDateTime(value.Value) : null;
        Transactions = await transactionService.GetTransactionsAsync(month);
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

    private async Task DeleteTransaction(TransactionResponse transaction)
    {
        var result = await _deleteConfirmationMessageBox.ShowAsync();
        if (result == true)
        {
            await transactionService.DeleteTransactionAsync(new(transaction.Id));
            Transactions = await transactionService.GetTransactionsAsync();
        }
    }
}
