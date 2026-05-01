using Bunit;
using FinanceSplit.Web.Components.Pages;
using TUnit.Core;

namespace FinanceSplit.Web.ComponentTests.Pages;

public class TransactionsComponentTests : ComponentTestBase
{
    [Test]
    public async Task TransactionsPage_RendersHeading()
    {
        var cut = RenderWithProviders<Transactions>();

        var markup = cut.Markup;
        await Assert.That(markup).Contains("Transactions");
    }

    [Test]
    public async Task TransactionsPage_RendersCreateForm()
    {
        var cut = RenderWithProviders<Transactions>();

        var markup = cut.Markup;
        await Assert.That(markup).Contains("Create Transaction");
        await Assert.That(markup).Contains("Title");
        await Assert.That(markup).Contains("Amount");
    }

    [Test]
    public async Task TransactionsPage_RendersSplitTypeOptions()
    {
        var cut = RenderWithProviders<Transactions>();

        var markup = cut.Markup;
        await Assert.That(markup).Contains("Split Type");
    }

    [Test]
    public async Task TransactionsPage_ShowsParticipantChips_WhenPeopleExist()
    {
        await MockApi.CreatePersonAsync("Alice");
        await MockApi.CreatePersonAsync("Bob");

        var cut = RenderWithProviders<Transactions>();

        var markup = cut.Markup;
        await Assert.That(markup).Contains("Alice");
        await Assert.That(markup).Contains("Bob");
        await Assert.That(markup).Contains("Participants");
    }

    [Test]
    public async Task TransactionsPage_ShowsPaidBySelect_WithPeople()
    {
        await MockApi.CreatePersonAsync("Alice");

        var cut = RenderWithProviders<Transactions>();

        var markup = cut.Markup;
        await Assert.That(markup).Contains("Paid By");
    }

    [Test]
    public async Task TransactionsPage_RenderCreateButton()
    {
        var cut = RenderWithProviders<Transactions>();

        var markup = cut.Markup;
        await Assert.That(markup).Contains("Create Transaction");
    }
}
