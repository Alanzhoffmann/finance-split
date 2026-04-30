using FinanceSplit.Data;
using FinanceSplit.Data.Repositories;
using FinanceSplit.Domain.Entities;
using FinanceSplit.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace FinanceSplit.Data.Tests;

public class TransactionRepositoryTests
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
        var personRepo = new PersonRepository(db);
        var txRepo = new TransactionRepository(db);

        var alice = new Person("Alice");
        await personRepo.AddAsync(alice);
        await personRepo.SaveChangesAsync();

        var tx = new Transaction("Groceries", 50m, alice, date: new DateTime(2026, 4, 15, 0, 0, 0, DateTimeKind.Utc));
        await txRepo.AddAsync(tx);
        await txRepo.SaveChangesAsync();

        var retrieved = await txRepo.GetByIdAsync(tx.Id);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.Title).IsEqualTo("Groceries");
        await Assert.That(retrieved.PaidBy.Name).IsEqualTo("Alice");
    }

    [Test]
    public async Task GetByMonthAsync_ShouldReturnOnlyMatchingMonth()
    {
        await using var db = CreateInMemoryContext();
        var personRepo = new PersonRepository(db);
        var txRepo = new TransactionRepository(db);

        var alice = new Person("Alice");
        await personRepo.AddAsync(alice);
        await personRepo.SaveChangesAsync();

        await txRepo.AddAsync(new Transaction("April", 10m, alice, date: new DateTime(2026, 4, 10, 0, 0, 0, DateTimeKind.Utc)));
        await txRepo.AddAsync(new Transaction("May", 20m, alice, date: new DateTime(2026, 5, 10, 0, 0, 0, DateTimeKind.Utc)));
        await txRepo.SaveChangesAsync();

        var aprilTx = await txRepo.GetByMonthAsync(new DateOnly(2026, 4, 1));

        await Assert.That(aprilTx.Count).IsEqualTo(1);
        await Assert.That(aprilTx[0].Title).IsEqualTo("April");
    }
}
