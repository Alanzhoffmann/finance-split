using FinanceSplit.Data;
using FinanceSplit.Data.Repositories;
using FinanceSplit.Domain.Entities;
using FinanceSplit.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace FinanceSplit.Data.Tests;

public class PersonRepositoryTests
{
    private static FinanceSplitDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<FinanceSplitDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        return new FinanceSplitDbContext(options);
    }

    [Test]
    public async Task AddAsync_And_GetByIdAsync_ShouldRoundTrip()
    {
        await using var db = CreateInMemoryContext();
        var repo = new PersonRepository(db);

        var person = new Person("Alice");
        await repo.AddAsync(person);
        await repo.SaveChangesAsync();

        var retrieved = await repo.GetByIdAsync(person.Id);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.Name).IsEqualTo("Alice");
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllPeople()
    {
        await using var db = CreateInMemoryContext();
        var repo = new PersonRepository(db);

        await repo.AddAsync(new Person("Bob"));
        await repo.AddAsync(new Person("Alice"));
        await repo.SaveChangesAsync();

        var all = await repo.GetAllAsync();

        await Assert.That(all.Count).IsEqualTo(2);
        await Assert.That(all[0].Name).IsEqualTo("Alice"); // ordered by name
    }

    [Test]
    public async Task GetByIdAsync_NotFound_ShouldReturnNull()
    {
        await using var db = CreateInMemoryContext();
        var repo = new PersonRepository(db);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        await Assert.That(result).IsNull();
    }
}
