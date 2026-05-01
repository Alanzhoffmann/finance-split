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
        var cut = Ctx.Render<Home>();

        var heading = cut.Find("h1");
        await Assert.That(heading.TextContent).IsEqualTo("Dashboard");
    }

    [Test]
    public async Task Dashboard_ShowsEmptyState_WhenNoPeople()
    {
        var cut = Ctx.Render<Home>();

        var message = cut.Find("p");
        await Assert.That(message.TextContent).Contains("No people added yet.");
    }

    [Test]
    public async Task Dashboard_HasMonthSelector()
    {
        var cut = Ctx.Render<Home>();

        var monthInput = cut.Find("input[type='month']");
        await Assert.That(monthInput).IsNotNull();
    }

    [Test]
    public async Task Dashboard_ShowsPersonCards_WhenPeopleExist()
    {
        // Pre-populate people via mock
        await MockApi.CreatePersonAsync("Alice");
        await MockApi.CreatePersonAsync("Bob");

        var cut = Ctx.Render<Home>();

        var cards = cut.FindAll(".person-card");
        await Assert.That(cards.Count).IsEqualTo(2);
    }

    [Test]
    public async Task Dashboard_PersonCard_ShowsName()
    {
        await MockApi.CreatePersonAsync("Alice");

        var cut = Ctx.Render<Home>();

        var cardHeading = cut.Find(".person-card h2");
        await Assert.That(cardHeading.TextContent).IsEqualTo("Alice");
    }

    [Test]
    public async Task Dashboard_PersonCard_ShowsFinancialFields()
    {
        await MockApi.CreatePersonAsync("Alice");

        var cut = Ctx.Render<Home>();

        var dts = cut.FindAll(".person-card dt");
        var labels = dts.Select(dt => dt.TextContent).ToList();
        await Assert.That(labels).Contains("Salary");
        await Assert.That(labels).Contains("Personal Expenses");
        await Assert.That(labels).Contains("Shared Expenses");
        await Assert.That(labels).Contains("Leftover");
    }
}
