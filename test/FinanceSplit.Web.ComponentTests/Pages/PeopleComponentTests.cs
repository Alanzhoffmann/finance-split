using Bunit;
using FinanceSplit.Web.Components.Pages;
using TUnit.Core;

namespace FinanceSplit.Web.ComponentTests.Pages;

public class PeopleComponentTests : ComponentTestBase
{
    [Test]
    public async Task PeoplePage_RendersHeading()
    {
        var cut = RenderWithProviders<People>();

        var markup = cut.Markup;
        await Assert.That(markup).Contains("People");
    }

    [Test]
    public async Task PeoplePage_RendersAddPersonSection()
    {
        var cut = RenderWithProviders<People>();

        var markup = cut.Markup;
        await Assert.That(markup).Contains("Add Person");
    }

    [Test]
    public async Task PeoplePage_ShowsTable_WhenPeopleExist()
    {
        await MockApi.CreatePersonAsync("Alice");

        var cut = RenderWithProviders<People>();

        var markup = cut.Markup;
        await Assert.That(markup).Contains("Alice");
        await Assert.That(markup).Contains("Name");
    }

    [Test]
    public async Task PeoplePage_ShowsCurrentSalary()
    {
        var person = await MockApi.CreatePersonAsync("Alice");
        await MockApi.AddSalaryAsync(person!.Id, new DateOnly(2026, 1, 1), 5000m);

        var cut = RenderWithProviders<People>();

        var markup = cut.Markup;
        await Assert.That(markup).Contains("5,000");
    }

    [Test]
    public async Task PeoplePage_ShowsSalaryHistoryCount()
    {
        var person = await MockApi.CreatePersonAsync("Bob");
        await MockApi.AddSalaryAsync(person!.Id, new DateOnly(2026, 1, 1), 4000m);
        await MockApi.AddSalaryAsync(person.Id, new DateOnly(2026, 2, 1), 4500m);

        var cut = RenderWithProviders<People>();

        var markup = cut.Markup;
        await Assert.That(markup).Contains("2 entries");
    }

    [Test]
    public async Task PeoplePage_ShowsNoSalaryMessage_WhenNone()
    {
        await MockApi.CreatePersonAsync("Charlie");

        var cut = RenderWithProviders<People>();

        var markup = cut.Markup;
        await Assert.That(markup).Contains("No salary records");
    }
}
