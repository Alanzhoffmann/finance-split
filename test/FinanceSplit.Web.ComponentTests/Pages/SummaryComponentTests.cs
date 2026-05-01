using Bunit;
using FinanceSplit.Contracts.Responses;
using FinanceSplit.Web.Components.Pages;
using TUnit.Core;

namespace FinanceSplit.Web.ComponentTests.Pages;

public class SummaryComponentTests : ComponentTestBase
{
    [Test]
    public async Task SummaryPage_RendersHeading()
    {
        var cut = RenderWithProviders<Summary>();

        var markup = cut.Markup;
        await Assert.That(markup).Contains("Monthly Summary");
    }

    [Test]
    public async Task SummaryPage_RendersDatePicker()
    {
        var cut = RenderWithProviders<Summary>();

        var markup = cut.Markup;
        await Assert.That(markup).Contains("Select Month");
    }

    [Test]
    public async Task SummaryPage_RendersLoadButton()
    {
        var cut = RenderWithProviders<Summary>();

        var markup = cut.Markup;
        await Assert.That(markup).Contains("Load");
    }

    [Test]
    public async Task SummaryPage_ShowsNoExpensesMessage_AfterLoad()
    {
        var cut = RenderWithProviders<Summary>();

        // Find the Load button by its text
        var buttons = cut.FindAll("button");
        var loadButton = buttons.First(b => b.TextContent.Contains("Load"));
        await cut.InvokeAsync(() => loadButton.Click());

        var markup = cut.Markup;
        await Assert.That(markup).Contains("No expenses for this month");
    }

    [Test]
    public async Task SummaryPage_DoesNotShowMessage_BeforeLoad()
    {
        var cut = RenderWithProviders<Summary>();

        var markup = cut.Markup;
        await Assert.That(markup).DoesNotContain("No expenses for this month");
    }
}
