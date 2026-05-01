using FinanceSplit.Domain.Entities;

namespace FinanceSplit.Domain.Repositories;

public interface IPersonRepository
{
    Task<Person?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Person>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Person person, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
