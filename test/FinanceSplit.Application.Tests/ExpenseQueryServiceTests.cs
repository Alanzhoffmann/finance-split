using FinanceSplit.Application.Queries;
using FinanceSplit.Application.Services;
using FinanceSplit.Contracts.Enums;
using FinanceSplit.Data;
using FinanceSplit.Data.Repositories;
using FinanceSplit.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace FinanceSplit.Application.Tests;

public class ExpenseQueryServiceTests
{
    private static (FinanceSplitDbContext db, ExpenseQueryService queryService, TransactionService txService, PersonService personService) CreateServices()
    {
        var options = new DbContextOptionsBuilder<FinanceSplitDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var db = new FinanceSplitDbContext(options);
        var personRepo = new PersonRepository(db);
        var txRepo = new TransactionRepository(db);
        return (db, new ExpenseQueryService(txRepo), new TransactionService(txRepo, personRepo), new PersonService(personRepo));
    }

    [Test]
    public async Task GetMonthlySummaryAsync_NoTransactions_ShouldReturnEmptySummary()
    {
        var (db, queryService, _, _) = CreateServices();
        await using var _ = db;

        var result = await queryService.GetMonthlySummaryAsync(new DateOnly(2026, 4, 1));

        await Assert.That(result.Transactions.Count).IsEqualTo(0);
    }

    [Test]
    public async Task GetSettlementsAsync_NoTransactions_ShouldReturnEmpty()
    {
        var (db, queryService, _, _) = CreateServices();
        await using var _ = db;

        var result = await queryService.GetSettlementsAsync(new DateOnly(2026, 4, 1));

        await Assert.That(result.Count).IsEqualTo(0);
    }

    [Test]
    public async Task GetSettlementsAsync_EvenSplit_ShouldReturnSettlement()
    {
        var (db, queryService, txService, personService) = CreateServices();
        await using var _ = db;

        var alice = await personService.CreatePersonAsync("Alice");
        var bob = await personService.CreatePersonAsync("Bob");

        // Add salaries so ratio/even split calculation works
        await personService.AddSalaryAsync(alice.Id, new DateOnly(2026, 4, 1), 5000m);
        await personService.AddSalaryAsync(bob.Id, new DateOnly(2026, 4, 1), 5000m);

        // Alice paid 100, split evenly between Alice and Bob
        await txService.CreateTransactionAsync(
            "Dinner",
            100m,
            alice.Id,
            SplitType.Even,
            [alice.Id, bob.Id],
            date: new DateTime(2026, 4, 15, 0, 0, 0, DateTimeKind.Utc)
        );

        var settlements = await queryService.GetSettlementsAsync(new DateOnly(2026, 4, 1));

        await Assert.That(settlements.Count).IsEqualTo(1);
        var settlement = settlements.First();
        await Assert.That(settlement.From.Name).IsEqualTo("Bob");
        await Assert.That(settlement.To.Name).IsEqualTo("Alice");
        await Assert.That(settlement.Amount).IsEqualTo(50m);
    }
}
