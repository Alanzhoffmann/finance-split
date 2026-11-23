using FinanceSplit.Contracts.Transactions.Commands;
using FinanceSplit.Contracts.Transactions.DTOs;

namespace FinanceSplit.Client.Interfaces;

public interface ITransactionDialogService
{
    Task<TransactionDto?> CreateTransactionAsync(CancellationToken cancellationToken = default);
    Task<TransactionDto?> EditTransactionAsync(
        TransactionDto transaction,
        CancellationToken cancellationToken = default
    );
    Task<bool> ShowDeleteTransactionConfirmationAsync(
        TransactionDto transaction,
        CancellationToken cancellationToken = default
    );
    Task<UpdateRecurrence?> GetRecurrenceUpdateTypeAsync(
        CancellationToken cancellationToken = default
    );
}
