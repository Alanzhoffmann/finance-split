using System.Net;
using System.Net.Http.Json;
using FinanceSplit.Contracts.Enums;
using FinanceSplit.Contracts.Requests;
using FinanceSplit.Contracts.Responses;
using FinanceSplit.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TUnit.AspNetCore;

namespace FinanceSplit.Api.Tests;

public class TransactionEndpointsTests : WebApplicationTest<FinanceSplitTestFactory, Program>
{
    protected override void ConfigureTestServices(IServiceCollection services)
    {
        services.AddDbContext<FinanceSplitDbContext>(options => options.UseInMemoryDatabase(GetIsolatedName("db")));
    }

    [Test]
    public async Task CreateTransaction_ShouldReturnCreated()
    {
        var client = Factory.CreateClient();

        // Create a person first
        var personResponse = await client.PostAsJsonAsync("/api/people", new CreatePersonRequest("Alice"));
        var person = await personResponse.Content.ReadFromJsonAsync<PersonResponse>();

        var request = new CreateTransactionRequest(
            "Groceries",
            50m,
            person!.Id,
            SplitType.None,
            [person.Id],
            new DateTime(2026, 4, 15, 0, 0, 0, DateTimeKind.Utc),
            null
        );

        var response = await client.PostAsJsonAsync("/api/transactions", request);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
        var tx = await response.Content.ReadFromJsonAsync<TransactionResponse>();
        await Assert.That(tx!.Title).IsEqualTo("Groceries");
        await Assert.That(tx.Amount).IsEqualTo(50m);
    }

    [Test]
    public async Task CreateTransaction_ShouldReturnBadRequest_WhenPaidByNotFound()
    {
        var client = Factory.CreateClient();

        var request = new CreateTransactionRequest(
            "Groceries",
            50m,
            Guid.NewGuid(),
            SplitType.None,
            [],
            new DateTime(2026, 4, 15, 0, 0, 0, DateTimeKind.Utc),
            null
        );

        var response = await client.PostAsJsonAsync("/api/transactions", request);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }
}
