using FinanceSplit.Contracts.Transactions.DTOs;

namespace FinanceSplit.Client.Interfaces;

public interface ITransactionDialogService
{
    Task<TransactionDto?> CreateTransactionAsync();
    Task<TransactionDto?> EditTransactionAsync(TransactionDto transaction);
    Task<bool> ShowDeleteTransactionConfirmationAsync(TransactionDto transaction);
}
