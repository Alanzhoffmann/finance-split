using TUnit.Core;

namespace FinanceSplit.Web.Tests.Pages;

public class TransactionsPageTests : WebPageTest
{
    [Test]
    public async Task TransactionsPage_DisplaysHeading()
    {
        await NavigateToAsync("/transactions");

        var heading = Page.GetByRole(Microsoft.Playwright.AriaRole.Heading, new() { Name = "Transactions" });
        await Assert.That(await heading.CountAsync()).IsGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task TransactionsPage_HasCreateForm()
    {
        await NavigateToAsync("/transactions");

        var titleInput = Page.GetByLabel("Title:");
        await Assert.That(await titleInput.CountAsync()).IsEqualTo(1);

        var amountInput = Page.GetByLabel("Amount:");
        await Assert.That(await amountInput.CountAsync()).IsEqualTo(1);

        var paidBySelect = Page.GetByLabel("Paid By:");
        await Assert.That(await paidBySelect.CountAsync()).IsEqualTo(1);

        var splitTypeSelect = Page.GetByLabel("Split Type:");
        await Assert.That(await splitTypeSelect.CountAsync()).IsEqualTo(1);

        var createButton = Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Create" });
        await Assert.That(await createButton.CountAsync()).IsEqualTo(1);
    }

    [Test]
    public async Task TransactionsPage_ShowsSplitTypeOptions()
    {
        await NavigateToAsync("/transactions");

        var splitSelect = Page.GetByLabel("Split Type:");
        var options = splitSelect.Locator("option");
        await Assert.That(await options.CountAsync()).IsEqualTo(3);
    }
}
