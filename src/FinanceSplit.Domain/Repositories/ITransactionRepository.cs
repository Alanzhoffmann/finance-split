using FinanceSplit.Domain.Entities;

namespace FinanceSplit.Domain.Repositories;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Transaction>> GetByMonthAsync(DateOnly month, CancellationToken ct = default);
    Task<IReadOnlyList<Transaction>> GetRecurringForMonthAsync(DateOnly month, CancellationToken ct = default);
    Task AddAsync(Transaction transaction, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
