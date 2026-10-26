using FinanceSplit.Contracts.Commands;
using FinanceSplit.Contracts.DTOs;

namespace FinanceSplit.Contracts.Services;

public interface ITransactionService
{
    Task<IReadOnlyList<TransactionResponse>?> GetTransactionsAsync(
        DateTime? month = null,
        CancellationToken cancellationToken = default
    );

    Task<TransactionResponse?> CreateTransactionAsync(
        CreateTransactionCommand command,
        CancellationToken cancellationToken = default
    );

    Task<TransactionResponse?> UpdateTransactionAsync(
        UpdateTransactionCommand command,
        CancellationToken cancellationToken = default
    );
}
