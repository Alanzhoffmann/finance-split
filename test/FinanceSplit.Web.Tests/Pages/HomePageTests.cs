using TUnit.Core;

namespace FinanceSplit.Web.Tests.Pages;

public class HomePageTests : WebPageTest
{
    [Test]
    public async Task HomePage_DisplaysTitle()
    {
        await NavigateToAsync("/");

        var title = await Page.TitleAsync();
        await Assert.That(title).Contains("FinanceSplit");
    }

    [Test]
    public async Task HomePage_HasNavigationLinks()
    {
        await NavigateToAsync("/");

        var peopleLink = Page.GetByRole(Microsoft.Playwright.AriaRole.Link, new() { Name = "People" });
        await Assert.That(await peopleLink.CountAsync()).IsGreaterThanOrEqualTo(1);

        var transactionsLink = Page.GetByRole(Microsoft.Playwright.AriaRole.Link, new() { Name = "Transactions" });
        await Assert.That(await transactionsLink.CountAsync()).IsGreaterThanOrEqualTo(1);

        var summaryLink = Page.GetByRole(Microsoft.Playwright.AriaRole.Link, new() { Name = "Summary" });
        await Assert.That(await summaryLink.CountAsync()).IsGreaterThanOrEqualTo(1);
    }
}
