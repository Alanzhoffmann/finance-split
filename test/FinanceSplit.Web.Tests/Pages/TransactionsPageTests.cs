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
    public async Task TransactionsPage_HasNewTransactionButton()
    {
        await NavigateToAsync("/transactions");

        var newButton = Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "New Transaction" });
        await Assert.That(await newButton.CountAsync()).IsEqualTo(1);
    }

    [Test]
    public async Task TransactionsPage_OpensCreateDialog()
    {
        await NavigateToAsync("/transactions");

        await Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "New Transaction" }).ClickAsync();

        var dialogTitle = Page.GetByText("New Transaction").First;
        await dialogTitle.WaitForAsync();
        await Assert.That(await dialogTitle.CountAsync()).IsGreaterThanOrEqualTo(1);
    }
}
