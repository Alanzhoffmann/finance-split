using FinanceSplit.Application.Services;
using FinanceSplit.Data;
using FinanceSplit.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinanceSplit.Application.Tests;

public class PersonServiceTests
{
    private static (FinanceSplitDbContext db, PersonService service) CreateService()
    {
        var options = new DbContextOptionsBuilder<FinanceSplitDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var db = new FinanceSplitDbContext(options);
        var repo = new PersonRepository(db);
        return (db, new PersonService(repo));
    }

    [Test]
    public async Task CreatePersonAsync_ShouldReturnPersonResponse()
    {
        var (db, service) = CreateService();
        await using var _ = db;

        var result = await service.CreatePersonAsync("Alice");

        await Assert.That(result.Name).IsEqualTo("Alice");
        await Assert.That(result.Id).IsNotEqualTo(Guid.Empty);
    }

    [Test]
    public async Task GetAllPeopleAsync_ShouldReturnAllCreated()
    {
        var (db, service) = CreateService();
        await using var _ = db;

        await service.CreatePersonAsync("Bob");
        await service.CreatePersonAsync("Alice");

        var all = await service.GetAllPeopleAsync();

        await Assert.That(all.Count).IsEqualTo(2);
    }

    [Test]
    public async Task GetPersonAsync_ShouldReturnNull_WhenNotFound()
    {
        var (db, service) = CreateService();
        await using var _ = db;

        var result = await service.GetPersonAsync(Guid.NewGuid());

        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task AddSalaryAsync_ShouldAddSalaryToPerson()
    {
        var (db, service) = CreateService();
        await using var _ = db;

        var person = await service.CreatePersonAsync("Alice");
        var result = await service.AddSalaryAsync(person.Id, new DateOnly(2026, 4, 1), 5000m);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Salaries.Count).IsEqualTo(1);
        await Assert.That(result.Salaries.First().Amount).IsEqualTo(5000m);
    }

    [Test]
    public async Task AddSalaryAsync_ShouldReturnNull_WhenPersonNotFound()
    {
        var (db, service) = CreateService();
        await using var _ = db;

        var result = await service.AddSalaryAsync(Guid.NewGuid(), new DateOnly(2026, 4, 1), 5000m);

        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task UpdateNameAsync_ShouldUpdateName()
    {
        var (db, service) = CreateService();
        await using var _ = db;

        var person = await service.CreatePersonAsync("Alice");
        var result = await service.UpdateNameAsync(person.Id, "Alicia");

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Name).IsEqualTo("Alicia");
    }

    [Test]
    public async Task UpdateNameAsync_ShouldReturnNull_WhenPersonNotFound()
    {
        var (db, service) = CreateService();
        await using var _ = db;

        var result = await service.UpdateNameAsync(Guid.NewGuid(), "Alicia");

        await Assert.That(result).IsNull();
    }
}
