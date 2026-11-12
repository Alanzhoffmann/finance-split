using FinanceSplit.Contracts.Commands;
using FinanceSplit.Contracts.DTOs;

namespace FinanceSplit.Contracts.Services;

public interface ITransactionService
{
    Task<IReadOnlyList<TransactionDto>?> GetTransactionsAsync(
        DateOnly? month = null,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<TransactionDto>> GetTransactionsAsync(
        IEnumerable<DateOnly> months,
        CancellationToken cancellationToken = default
    ) => Task.FromResult<IReadOnlyList<TransactionDto>>([]);

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
