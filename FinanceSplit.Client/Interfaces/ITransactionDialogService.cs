using FinanceSplit.Contracts.DTOs;

namespace FinanceSplit.Client.Interfaces;

public interface ITransactionDialogService
{
    Task<TransactionResponse?> CreateTransactionAsync();
    Task<TransactionResponse?> EditTransactionAsync(TransactionResponse transaction);
    Task<bool> ShowDeleteTransactionConfirmationAsync(TransactionResponse transaction);
}
