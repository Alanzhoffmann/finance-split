using FinanceSplit.Contracts.Transactions.Commands;
using FinanceSplit.Contracts.Transactions.Dtos;

namespace FinanceSplit.Contracts.Services;

public interface ITransactionService
{
    Task<IReadOnlyList<TransactionDto>?> GetTransactionsAsync(
        DateOnly? month = null,
        CancellationToken cancellationToken = default
    );

    async Task<IReadOnlyList<TransactionDto>> GetTransactionsAsync(
        IEnumerable<DateOnly> months,
        CancellationToken cancellationToken = default
    )
    {
        var allTransactions = new List<TransactionDto>();
        foreach (var month in months)
        {
            var monthTransactions = await GetTransactionsAsync(month, cancellationToken);
            if (monthTransactions is not null)
            {
                allTransactions.AddRange();
            }
        }

        return allTransactions;
    }

    Task<TransactionDto?> CreateTransactionAsync(
        CreateTransactionCommand command,
        CancellationToken cancellationToken = default
    );

    Task<TransactionDto?> CreateRecurringTransactionAsync(
        CreateRecurringTransactionCommand command,
        CancellationToken cancellationToken = default
    );

    Task<TransactionDto?> CreateSplitTransactionAsync(
        CreateSplitTransactionCommand command,
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
