using Bunit;
using FinanceSplit.Contracts.Enums;
using FinanceSplit.Contracts.Requests;
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
    public async Task TransactionsPage_RendersNewTransactionButton()
    {
        var cut = RenderWithProviders<Transactions>();

        var markup = cut.Markup;
        await Assert.That(markup).Contains("New Transaction");
    }

    [Test]
    public async Task TransactionsPage_RendersMonthPicker()
    {
        var cut = RenderWithProviders<Transactions>();

        var markup = cut.Markup;
        await Assert.That(markup).Contains("Month");
    }

    [Test]
    public async Task TransactionsPage_ShowsNoTransactionsMessage_WhenEmpty()
    {
        var cut = RenderWithProviders<Transactions>();

        var markup = cut.Markup;
        await Assert.That(markup).Contains("No transactions for this month");
    }

    [Test]
    public async Task TransactionsPage_ShowsTransactionTable_WhenDataExists()
    {
        var alice = await MockApi.CreatePersonAsync("Alice");
        var request = new CreateTransactionRequest("Groceries", 50m, alice!.Id, SplitType.None, [alice.Id], DateTime.UtcNow);
        await MockApi.CreateTransactionAsync(request);

        var cut = RenderWithProviders<Transactions>();

        var markup = cut.Markup;
        await Assert.That(markup).Contains("Groceries");
        await Assert.That(markup).Contains("Alice");
    }

    [Test]
    public async Task TransactionsPage_ShowsEditButton_ForTransaction()
    {
        var alice = await MockApi.CreatePersonAsync("Alice");
        var request = new CreateTransactionRequest("Dinner", 30m, alice!.Id, SplitType.None, [alice.Id], DateTime.UtcNow);
        await MockApi.CreateTransactionAsync(request);

        var cut = RenderWithProviders<Transactions>();

        // Edit icon button should be rendered in the Actions column
        var editButtons = cut.FindAll("button.mud-icon-button");
        await Assert.That(editButtons.Count).IsGreaterThanOrEqualTo(1);
    }
}
