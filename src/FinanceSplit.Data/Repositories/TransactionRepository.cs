using FinanceSplit.Domain.Entities;
using FinanceSplit.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinanceSplit.Data.Repositories;

public class TransactionRepository(FinanceSplitDbContext db) : ITransactionRepository
{
    public async Task<Transaction?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Transactions.Include(t => t.PaidBy).Include(t => t.Participants).FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IReadOnlyList<Transaction>> GetByMonthAsync(DateOnly month, CancellationToken ct = default)
    {
        var start = new DateTime(month.Year, month.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = start.AddMonths(1);

        return await db
            .Transactions.Include(t => t.PaidBy)
            .Include(t => t.Participants)
            .Where(t => t.Recurrence == null && t.Date >= start && t.Date < end)
            .OrderBy(t => t.Date)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Transaction>> GetRecurringForMonthAsync(DateOnly month, CancellationToken ct = default)
    {
        // Load all recurring transactions and filter in memory (recurrence logic is in the domain)
        var recurring = await db.Transactions.Include(t => t.PaidBy).Include(t => t.Participants).Where(t => t.Recurrence != null).ToListAsync(ct);

        return recurring.Where(t => t.Recurrence!.OccursIn(month)).ToList();
    }

    public async Task AddAsync(Transaction transaction, CancellationToken ct = default) => await db.Transactions.AddAsync(transaction, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
