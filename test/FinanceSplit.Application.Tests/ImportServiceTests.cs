using FinanceSplit.Application.Services;
using FinanceSplit.Contracts.Enums;
using FinanceSplit.Contracts.Requests;
using FinanceSplit.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceSplit.Application.Tests;

public class ImportServiceTests
{
    private static (FinanceSplitDbContext db, ImportService service) CreateService()
    {
        var options = new DbContextOptionsBuilder<FinanceSplitDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var db = new FinanceSplitDbContext(options);
        return (db, new ImportService(db));
    }

    [Test]
    public async Task ImportAsync_ShouldThrow_WhenFormatVersionUnsupported()
    {
        var (db, service) = CreateService();
        await using var _ = db;

        var request = new ImportBackupRequest("unknown-version", DateTimeOffset.UtcNow, new ImportPayload([], []));

        await Assert.That(async () => await service.ImportAsync(request)).Throws<ArgumentException>();
    }

    [Test]
    public async Task ImportAsync_ShouldImportPeople()
    {
        var (db, service) = CreateService();
        await using var _ = db;

        var personId = Guid.NewGuid();
        var request = new ImportBackupRequest(
            "finances-split-backup-v1",
            DateTimeOffset.UtcNow,
            new ImportPayload([new ImportPerson(personId, "Alice", [])], [])
        );

        var result = await service.ImportAsync(request);

        await Assert.That(result.PeopleImported).IsEqualTo(1);
        await Assert.That(db.People.Count()).IsEqualTo(1);
    }

    [Test]
    public async Task ImportAsync_ShouldDeduplicatePeopleByName()
    {
        var (db, service) = CreateService();
        await using var _ = db;

        // Import Alice first
        var request1 = new ImportBackupRequest(
            "finances-split-backup-v1",
            DateTimeOffset.UtcNow,
            new ImportPayload([new ImportPerson(Guid.NewGuid(), "Alice", [])], [])
        );
        await service.ImportAsync(request1);

        // Import again with same name (different case)
        var request2 = new ImportBackupRequest(
            "finances-split-backup-v1",
            DateTimeOffset.UtcNow,
            new ImportPayload([new ImportPerson(Guid.NewGuid(), "alice", [])], [])
        );
        await service.ImportAsync(request2);

        await Assert.That(db.People.Count()).IsEqualTo(1);
    }

    [Test]
    public async Task ImportAsync_ShouldImportSalaries()
    {
        var (db, service) = CreateService();
        await using var _ = db;

        var personId = Guid.NewGuid();
        var request = new ImportBackupRequest(
            "finances-split-backup-v1",
            DateTimeOffset.UtcNow,
            new ImportPayload([new ImportPerson(personId, "Alice", [new ImportSalaryRecord(Guid.NewGuid(), 4, 2026, 5000m)])], [])
        );

        var result = await service.ImportAsync(request);

        await Assert.That(result.SalaryRecordsImported).IsEqualTo(1);
        var person = await db.People.FirstAsync();
        await Assert.That(person.Salaries.Count()).IsEqualTo(1);
    }

    [Test]
    public async Task ImportAsync_ShouldDeduplicateSalariesByDate()
    {
        var (db, service) = CreateService();
        await using var _ = db;

        var personId = Guid.NewGuid();
        var salary = new ImportSalaryRecord(Guid.NewGuid(), 4, 2026, 5000m);
        var request = new ImportBackupRequest(
            "finances-split-backup-v1",
            DateTimeOffset.UtcNow,
            new ImportPayload([new ImportPerson(personId, "Alice", [salary])], [])
        );

        await service.ImportAsync(request);
        // Import again with same salary date
        await service.ImportAsync(request);

        var person = await db.People.FirstAsync();
        await Assert.That(person.Salaries.Count()).IsEqualTo(1);
    }

    [Test]
    public async Task ImportAsync_ShouldImportTransactions()
    {
        var (db, service) = CreateService();
        await using var _ = db;

        var personId = Guid.NewGuid();
        var request = new ImportBackupRequest(
            "finances-split-backup-v1",
            DateTimeOffset.UtcNow,
            new ImportPayload(
                [new ImportPerson(personId, "Alice", [])],
                [
                    new ImportTransaction(
                        Guid.NewGuid(),
                        "Groceries",
                        50m,
                        4,
                        2026,
                        SplitType.None,
                        personId,
                        false,
                        [new ImportTransactionResponsibility(Guid.NewGuid(), personId)]
                    ),
                ]
            )
        );

        var result = await service.ImportAsync(request);

        await Assert.That(result.TransactionsImported).IsEqualTo(1);
        await Assert.That(db.Transactions.Count()).IsEqualTo(1);
    }

    [Test]
    public async Task ImportAsync_ShouldSkipTransaction_WhenPaidByNotFound()
    {
        var (db, service) = CreateService();
        await using var _ = db;

        var unknownPersonId = Guid.NewGuid();
        var request = new ImportBackupRequest(
            "finances-split-backup-v1",
            DateTimeOffset.UtcNow,
            new ImportPayload(
                [new ImportPerson(Guid.NewGuid(), "Alice", [])],
                [new ImportTransaction(Guid.NewGuid(), "Groceries", 50m, 4, 2026, SplitType.None, unknownPersonId, false, [])]
            )
        );

        var result = await service.ImportAsync(request);

        await Assert.That(result.TransactionsImported).IsEqualTo(0);
    }

    [Test]
    public async Task ImportAsync_ShouldDeduplicateTransactions()
    {
        var (db, service) = CreateService();
        await using var _ = db;

        var personId = Guid.NewGuid();
        var tx = new ImportTransaction(
            Guid.NewGuid(),
            "Groceries",
            50m,
            4,
            2026,
            SplitType.None,
            personId,
            false,
            [new ImportTransactionResponsibility(Guid.NewGuid(), personId)]
        );

        var request = new ImportBackupRequest(
            "finances-split-backup-v1",
            DateTimeOffset.UtcNow,
            new ImportPayload([new ImportPerson(personId, "Alice", [])], [tx])
        );

        await service.ImportAsync(request);
        await service.ImportAsync(request);

        await Assert.That(db.Transactions.Count()).IsEqualTo(1);
    }

    [Test]
    public async Task ImportAsync_ShouldHandleRecurringTransaction()
    {
        var (db, service) = CreateService();
        await using var _ = db;

        var personId = Guid.NewGuid();
        var request = new ImportBackupRequest(
            "finances-split-backup-v1",
            DateTimeOffset.UtcNow,
            new ImportPayload(
                [new ImportPerson(personId, "Alice", [])],
                [
                    new ImportTransaction(
                        Guid.NewGuid(),
                        "Rent",
                        1000m,
                        4,
                        2026,
                        SplitType.Even,
                        personId,
                        true,
                        [new ImportTransactionResponsibility(Guid.NewGuid(), personId)]
                    ),
                ]
            )
        );

        var result = await service.ImportAsync(request);

        await Assert.That(result.TransactionsImported).IsEqualTo(1);
        var tx = await db.Transactions.FirstAsync();
        await Assert.That(tx.Recurrence).IsNotNull();
    }
}
