using FinanceSplit.Contracts.Transactions.Dtos;

namespace FinanceSplit.Client.Interfaces;

public interface ITransactionDialogService
{
    Task<TransactionDto?> CreateTransactionAsync();
    Task<TransactionDto?> EditTransactionAsync(TransactionDto transaction);
    Task<bool> ShowDeleteTransactionConfirmationAsync(TransactionDto transaction);
}
