using FinanceSplit.Application.Services;
using FinanceSplit.Contracts.Enums;
using FinanceSplit.Data;
using FinanceSplit.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinanceSplit.Application.Tests;

public class TransactionServiceTests
{
    private static (FinanceSplitDbContext db, TransactionService txService, PersonService personService) CreateServices()
    {
        var options = new DbContextOptionsBuilder<FinanceSplitDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var db = new FinanceSplitDbContext(options);
        var personRepo = new PersonRepository(db);
        var txRepo = new TransactionRepository(db);
        return (db, new TransactionService(txRepo, personRepo), new PersonService(personRepo));
    }

    [Test]
    public async Task CreateTransactionAsync_ShouldReturnTransactionResponse()
    {
        var (db, txService, personService) = CreateServices();
        await using var _ = db;

        var alice = await personService.CreatePersonAsync("Alice");

        var result = await txService.CreateTransactionAsync(
            "Groceries",
            50m,
            alice.Id,
            SplitType.None,
            [alice.Id],
            date: new DateTime(2026, 4, 15, 0, 0, 0, DateTimeKind.Utc)
        );

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Title).IsEqualTo("Groceries");
        await Assert.That(result.Amount).IsEqualTo(50m);
    }

    [Test]
    public async Task CreateTransactionAsync_EvenSplit_ShouldSetSplitType()
    {
        var (db, txService, personService) = CreateServices();
        await using var _ = db;

        var alice = await personService.CreatePersonAsync("Alice");
        var bob = await personService.CreatePersonAsync("Bob");

        var result = await txService.CreateTransactionAsync(
            "Dinner",
            100m,
            alice.Id,
            SplitType.Even,
            [alice.Id, bob.Id],
            date: new DateTime(2026, 4, 15, 0, 0, 0, DateTimeKind.Utc)
        );

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.SplitType).IsEqualTo(SplitType.Even);
    }

    [Test]
    public async Task CreateTransactionAsync_ShouldReturnNull_WhenPaidByNotFound()
    {
        var (db, txService, _) = CreateServices();
        await using var _ = db;

        var result = await txService.CreateTransactionAsync(
            "Groceries",
            50m,
            Guid.NewGuid(),
            SplitType.None,
            [],
            date: new DateTime(2026, 4, 15, 0, 0, 0, DateTimeKind.Utc)
        );

        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task CreateTransactionAsync_ShouldReturnNull_WhenParticipantNotFound()
    {
        var (db, txService, personService) = CreateServices();
        await using var _ = db;

        var alice = await personService.CreatePersonAsync("Alice");

        var result = await txService.CreateTransactionAsync(
            "Dinner",
            100m,
            alice.Id,
            SplitType.Even,
            [alice.Id, Guid.NewGuid()],
            date: new DateTime(2026, 4, 15, 0, 0, 0, DateTimeKind.Utc)
        );

        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task GetTransactionAsync_ShouldReturnTransaction()
    {
        var (db, txService, personService) = CreateServices();
        await using var _ = db;

        var alice = await personService.CreatePersonAsync("Alice");
        var created = await txService.CreateTransactionAsync(
            "Groceries",
            50m,
            alice.Id,
            SplitType.None,
            [alice.Id],
            date: new DateTime(2026, 4, 15, 0, 0, 0, DateTimeKind.Utc)
        );

        var retrieved = await txService.GetTransactionAsync(created!.Id);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.Title).IsEqualTo("Groceries");
    }

    [Test]
    public async Task GetTransactionAsync_ShouldReturnNull_WhenNotFound()
    {
        var (db, txService, _) = CreateServices();
        await using var _ = db;

        var result = await txService.GetTransactionAsync(Guid.NewGuid());

        await Assert.That(result).IsNull();
    }
}
