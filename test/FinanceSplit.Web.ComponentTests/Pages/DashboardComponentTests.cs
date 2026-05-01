using Bunit;
using FinanceSplit.Contracts.Responses;
using FinanceSplit.Web.Components.Pages;
using TUnit.Core;

namespace FinanceSplit.Web.ComponentTests.Pages;

public class DashboardComponentTests : ComponentTestBase
{
    [Test]
    public async Task Dashboard_RendersHeading()
    {
        var cut = RenderWithProviders<Home>();

        var markup = cut.Markup;
        await Assert.That(markup).Contains("Dashboard");
    }

    [Test]
    public async Task Dashboard_ShowsEmptyState_WhenNoPeople()
    {
        var cut = RenderWithProviders<Home>();

        var markup = cut.Markup;
        await Assert.That(markup).Contains("No people added yet.");
    }

    [Test]
    public async Task Dashboard_ShowsPersonCards_WhenPeopleExist()
    {
        await MockApi.CreatePersonAsync("Alice");
        await MockApi.CreatePersonAsync("Bob");

        var cut = RenderWithProviders<Home>();

        var cards = cut.FindAll(".person-card");
        await Assert.That(cards.Count).IsEqualTo(2);
    }

    [Test]
    public async Task Dashboard_PersonCard_ShowsName()
    {
        await MockApi.CreatePersonAsync("Alice");

        var cut = RenderWithProviders<Home>();

        var markup = cut.Markup;
        await Assert.That(markup).Contains("Alice");
    }

    [Test]
    public async Task Dashboard_PersonCard_ShowsFinancialLabels()
    {
        await MockApi.CreatePersonAsync("Alice");

        var cut = RenderWithProviders<Home>();

        var markup = cut.Markup;
        await Assert.That(markup).Contains("Salary");
        await Assert.That(markup).Contains("Personal Expenses");
        await Assert.That(markup).Contains("Shared Expenses");
        await Assert.That(markup).Contains("Leftover");
    }

    [Test]
    public async Task Dashboard_ShowsLinkToPeople_WhenEmpty()
    {
        var cut = RenderWithProviders<Home>();

        var link = cut.Find("a[href='/people']");
        await Assert.That(link.TextContent).Contains("Add people");
    }
}
