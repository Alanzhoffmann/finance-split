using TUnit.Core;

namespace FinanceSplit.Web.Tests.Pages;

public class SummaryPageTests : WebPageTest
{
    [Test]
    public async Task SummaryPage_DisplaysHeading()
    {
        await NavigateToAsync("/summary");

        var heading = Page.GetByRole(Microsoft.Playwright.AriaRole.Heading, new() { Name = "Monthly Summary" });
        await Assert.That(await heading.CountAsync()).IsEqualTo(1);
    }

    [Test]
    public async Task SummaryPage_HasMonthSelector()
    {
        await NavigateToAsync("/summary");

        var monthInput = Page.Locator("input[type='month']");
        await Assert.That(await monthInput.CountAsync()).IsEqualTo(1);
    }

    [Test]
    public async Task SummaryPage_HasLoadButton()
    {
        await NavigateToAsync("/summary");

        var loadButton = Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Load" });
        await Assert.That(await loadButton.CountAsync()).IsEqualTo(1);
    }

    [Test]
    public async Task SummaryPage_ShowsNoExpensesMessage_WhenNoData()
    {
        await NavigateToAsync("/summary");

        await Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Load" }).ClickAsync();

        var noExpenses = Page.GetByText("No expenses for this month.");
        await noExpenses.WaitForAsync();
        await Assert.That(await noExpenses.CountAsync()).IsEqualTo(1);
    }
}
