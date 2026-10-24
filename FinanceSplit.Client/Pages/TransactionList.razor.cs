using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Components;

namespace FinanceSplit.Client.Pages;

public partial class TransactionList(ITransactionService transactionService)
{
    [PersistentState(AllowUpdates = true)]
    public IReadOnlyList<TransactionResponse>? Transactions { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Transactions = await transactionService.GetTransactionsAsync();
    }
}
