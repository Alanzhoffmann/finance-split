using FinanceSplit.Domain.Entities;
using FinanceSplit.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinanceSplit.Data.Repositories;

public class PersonRepository(FinanceSplitDbContext db) : IPersonRepository
{
    public async Task<Person?> GetByIdAsync(Guid id, CancellationToken ct = default) => await db.People.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IReadOnlyList<Person>> GetAllAsync(CancellationToken ct = default) => await db.People.OrderBy(p => p.Name).ToListAsync(ct);

    public async Task AddAsync(Person person, CancellationToken ct = default) => await db.People.AddAsync(person, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
