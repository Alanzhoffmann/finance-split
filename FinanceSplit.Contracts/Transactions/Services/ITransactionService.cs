using FinanceSplit.Contracts.Transactions.Commands;
using FinanceSplit.Contracts.Transactions.DTOs;

namespace FinanceSplit.Contracts.Transactions.Services;

public interface ITransactionService
{
    Task<IReadOnlyList<TransactionDto>?> GetTransactionsAsync(
        DateOnly? month = null,
        CancellationToken cancellationToken = default
    );

    Task<TransactionDto?> CreateTransactionAsync(
        CreateTransactionCommand command,
        CancellationToken cancellationToken = default
    );

    Task<TransactionDto?> UpdateTransactionAsync(
        UpdateTransactionCommand command,
        CancellationToken cancellationToken = default
    );

    Task DeleteTransactionAsync(
        DeleteTransactionCommand command,
        CancellationToken cancellationToken = default
    );
}
