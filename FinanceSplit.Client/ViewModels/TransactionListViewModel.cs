using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceSplit.Client.Interfaces;
using FinanceSplit.Contracts.Services;
using FinanceSplit.Contracts.Transactions.Dtos;
using Microsoft.AspNetCore.Components;

namespace FinanceSplit.Client.ViewModels;

public partial class TransactionListViewModel(
    ITransactionService transactionService,
    ITransactionDialogService dialogService,
    NavigationManager navigationManager,
    TimeProvider timeProvider
) : ObservableObject
{
    public const string MonthParamName = "month";

    [ObservableProperty]
    public partial DateOnly CurrentMonth { get; set; }

    [PersistentState(AllowUpdates = true)]
    public List<TransactionDto>? Transactions { get; private set; }

    [MemberNotNullWhen(true, nameof(Transactions))]
    public bool IsLoaded => Transactions is not null;

    public async Task InitializeAsync(DateOnly? targetMonth = null)
    {
        if (targetMonth is null)
        {
            var today = timeProvider.GetLocalNow().Date;
            targetMonth = new DateOnly(today.Year, today.Month, 1);
        }

        if (CurrentMonth == targetMonth && IsLoaded)
        {
            return;
        }

        CurrentMonth = targetMonth.Value;
        Transactions = null;

        var data = await transactionService.GetTransactionsAsync(CurrentMonth);

        if (data is null)
        {
            return;
        }

        Transactions = [.. data];
    }

    public void ChangeMonth(DateOnly newMonth)
    {
        if (newMonth == CurrentMonth)
        {
            return;
        }

        var newUri = navigationManager.GetUriWithQueryParameter(
            MonthParamName,
            $"{newMonth:yyyy-MM}"
        );
        navigationManager.NavigateTo(newUri);
    }

    [RelayCommand]
    private async Task CreateTransactionAsync()
    {
        var newTransaction = await dialogService.CreateTransactionAsync();

        if (newTransaction is not null)
        {
            await UpdateTransactionListAsync(newTransaction);
        }
    }

    [RelayCommand]
    private async Task EditTransactionAsync(TransactionDto transaction)
    {
        var editedTransaction = await dialogService.EditTransactionAsync(transaction);

        if (editedTransaction is not null)
        {
            await UpdateTransactionListAsync(editedTransaction);
        }
    }

    [RelayCommand]
    private async Task DeleteTransactionAsync(TransactionDto transaction)
    {
        var result = await dialogService.ShowDeleteTransactionConfirmationAsync(transaction);

        if (result)
        {
            await transactionService.DeleteTransactionAsync(new(transaction.Id));

            if (Transactions is not null)
            {
                var index = Transactions.FindIndex(t => t.Id == transaction.Id);
                if (index != -1)
                {
                    Transactions.RemoveAt(index);
                }
            }
            else
            {
                await RefreshTransactionAsync();
            }
        }
    }

    private async Task UpdateTransactionListAsync(TransactionDto transaction)
    {
        if (Transactions is not null)
        {
            var index = Transactions.FindIndex(t => t.Id == transaction.Id);
            if (index != -1)
            {
                Transactions[index] = transaction;
            }
            else
            {
                Transactions.Add(transaction);
                Transactions.Sort((x, y) => x.Name.CompareTo(y.Name));
            }
        }
        else
        {
            await RefreshTransactionAsync();
        }
    }

    private async Task RefreshTransactionAsync()
    {
        var data = await transactionService.GetTransactionsAsync(CurrentMonth);

        if (data is null)
        {
            return;
        }

        Transactions = [.. data];
    }
}
