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

public class ExpenseEndpointsTests : WebApplicationTest<FinanceSplitTestFactory, Program>
{
    protected override void ConfigureTestServices(IServiceCollection services)
    {
        services.AddDbContext<FinanceSplitDbContext>(options => options.UseInMemoryDatabase(GetIsolatedName("db")));
    }

    [Test]
    public async Task GetSummary_ShouldReturnEmptySummary_WhenNoTransactions()
    {
        var client = Factory.CreateClient();

        var response = await client.GetAsync("/api/expenses/summary/2026/4");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        var summary = await response.Content.ReadFromJsonAsync<MonthlyExpenseSummaryResponse>();
        await Assert.That(summary!.Transactions.Count).IsEqualTo(0);
    }

    [Test]
    public async Task GetSettlements_ShouldReturnEmpty_WhenNoTransactions()
    {
        var client = Factory.CreateClient();

        var response = await client.GetAsync("/api/expenses/settlements/2026/4");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        var settlements = await response.Content.ReadFromJsonAsync<List<ExpenseSettlementResponse>>();
        await Assert.That(settlements!.Count).IsEqualTo(0);
    }

    [Test]
    public async Task GetSettlements_ShouldReturnSettlement_WhenEvenSplitExists()
    {
        var client = Factory.CreateClient();

        // Create people
        var aliceResp = await client.PostAsJsonAsync("/api/people", new CreatePersonRequest("Alice"));
        var alice = await aliceResp.Content.ReadFromJsonAsync<PersonResponse>();

        var bobResp = await client.PostAsJsonAsync("/api/people", new CreatePersonRequest("Bob"));
        var bob = await bobResp.Content.ReadFromJsonAsync<PersonResponse>();

        // Add salaries
        await client.PostAsJsonAsync($"/api/people/{alice!.Id}/salaries", new AddSalaryRequest(new DateOnly(2026, 4, 1), 5000m));
        await client.PostAsJsonAsync($"/api/people/{bob!.Id}/salaries", new AddSalaryRequest(new DateOnly(2026, 4, 1), 5000m));

        // Create even split transaction
        var txRequest = new CreateTransactionRequest(
            "Dinner",
            100m,
            alice.Id,
            SplitType.Even,
            [alice.Id, bob.Id],
            new DateTime(2026, 4, 15, 0, 0, 0, DateTimeKind.Utc),
            null
        );
        await client.PostAsJsonAsync("/api/transactions", txRequest);

        // Get settlements
        var response = await client.GetAsync("/api/expenses/settlements/2026/4");
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

        var settlements = await response.Content.ReadFromJsonAsync<List<ExpenseSettlementResponse>>();
        await Assert.That(settlements!.Count).IsEqualTo(1);
        await Assert.That(settlements[0].From.Name).IsEqualTo("Bob");
        await Assert.That(settlements[0].To.Name).IsEqualTo("Alice");
        await Assert.That(settlements[0].Amount).IsEqualTo(50m);
    }
}
