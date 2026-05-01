using System.Net;
using System.Net.Http.Json;
using FinanceSplit.Contracts.Requests;
using FinanceSplit.Contracts.Responses;
using FinanceSplit.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TUnit.AspNetCore;

namespace FinanceSplit.Api.Tests;

public class PersonEndpointsTests : WebApplicationTest<FinanceSplitTestFactory, Program>
{
    protected override void ConfigureTestServices(IServiceCollection services)
    {
        services.AddDbContext<FinanceSplitDbContext>(options => options.UseInMemoryDatabase(GetIsolatedName("db")));
    }

    [Test]
    public async Task GetAll_ShouldReturnEmptyList_Initially()
    {
        var client = Factory.CreateClient();

        var response = await client.GetAsync("/api/people");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        var people = await response.Content.ReadFromJsonAsync<List<PersonResponse>>();
        await Assert.That(people!.Count).IsEqualTo(0);
    }

    [Test]
    public async Task CreatePerson_ShouldReturnCreated()
    {
        var client = Factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/people", new CreatePersonRequest("Alice"));

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
        var person = await response.Content.ReadFromJsonAsync<PersonResponse>();
        await Assert.That(person!.Name).IsEqualTo("Alice");
        await Assert.That(person.Id).IsNotEqualTo(Guid.Empty);
    }

    [Test]
    public async Task GetById_ShouldReturnPerson_AfterCreate()
    {
        var client = Factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/people", new CreatePersonRequest("Bob"));
        var created = await createResponse.Content.ReadFromJsonAsync<PersonResponse>();

        var getResponse = await client.GetAsync($"/api/people/{created!.Id}");

        await Assert.That(getResponse.StatusCode).IsEqualTo(HttpStatusCode.OK);
        var person = await getResponse.Content.ReadFromJsonAsync<PersonResponse>();
        await Assert.That(person!.Name).IsEqualTo("Bob");
    }

    [Test]
    public async Task GetById_ShouldReturn404_WhenNotFound()
    {
        var client = Factory.CreateClient();

        var response = await client.GetAsync($"/api/people/{Guid.NewGuid()}");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task AddSalary_ShouldReturnUpdatedPerson()
    {
        var client = Factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/people", new CreatePersonRequest("Alice"));
        var created = await createResponse.Content.ReadFromJsonAsync<PersonResponse>();

        var salaryResponse = await client.PostAsJsonAsync($"/api/people/{created!.Id}/salaries", new AddSalaryRequest(new DateOnly(2026, 4, 1), 5000m));

        await Assert.That(salaryResponse.StatusCode).IsEqualTo(HttpStatusCode.OK);
        var updated = await salaryResponse.Content.ReadFromJsonAsync<PersonResponse>();
        await Assert.That(updated!.Salaries.Count).IsEqualTo(1);
    }

    [Test]
    public async Task UpdateName_ShouldReturnUpdatedPerson()
    {
        var client = Factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/people", new CreatePersonRequest("Alice"));
        var created = await createResponse.Content.ReadFromJsonAsync<PersonResponse>();

        var updateResponse = await client.PutAsJsonAsync($"/api/people/{created!.Id}/name", new UpdatePersonNameRequest("Alicia"));

        await Assert.That(updateResponse.StatusCode).IsEqualTo(HttpStatusCode.OK);
        var updated = await updateResponse.Content.ReadFromJsonAsync<PersonResponse>();
        await Assert.That(updated!.Name).IsEqualTo("Alicia");
    }
}
