using TUnit.Core;

namespace FinanceSplit.Web.Tests.Pages;

public class HomePageTests : WebPageTest
{
    [Test]
    public async Task HomePage_DisplaysTitle()
    {
        await NavigateToAsync("/");

        var title = await Page.TitleAsync();
        await Assert.That(title).Contains("Finance Split");
    }

    [Test]
    public async Task HomePage_HasNavigationDrawer()
    {
        await NavigateToAsync("/");

        var peopleLink = Page.GetByText("People");
        await Assert.That(await peopleLink.CountAsync()).IsGreaterThanOrEqualTo(1);

        var transactionsLink = Page.GetByText("Transactions");
        await Assert.That(await transactionsLink.CountAsync()).IsGreaterThanOrEqualTo(1);

        var summaryLink = Page.GetByText("Monthly Summary");
        await Assert.That(await summaryLink.CountAsync()).IsGreaterThanOrEqualTo(1);
    }
}
