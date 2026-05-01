using TUnit.Core;

namespace FinanceSplit.Web.Tests.Pages;

public class DashboardPageTests : WebPageTest
{
    [Test]
    public async Task Dashboard_DisplaysHeading()
    {
        await NavigateToAsync("/");

        var heading = Page.GetByText("Dashboard").First;
        await heading.WaitForAsync();
        await Assert.That(await heading.CountAsync()).IsGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task Dashboard_ShowsEmptyState_WhenNoPeople()
    {
        await NavigateToAsync("/");

        var message = Page.GetByText("No people added yet.");
        await message.WaitForAsync();
        await Assert.That(await message.CountAsync()).IsEqualTo(1);
    }

    [Test]
    public async Task Dashboard_ShowsPersonCard_AfterPersonAdded()
    {
        await NavigateToAsync("/people");
        await Page.GetByLabel("Name").FillAsync("Alice");
        await Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Add", Exact = true }).ClickAsync();

        var aliceText = Page.GetByText("Alice");
        await aliceText.First.WaitForAsync();

        await NavigateToAsync("/");

        var card = Page.Locator(".person-card");
        await card.WaitForAsync();
        await Assert.That(await card.CountAsync()).IsEqualTo(1);
    }

    [Test]
    public async Task Dashboard_ShowsFinancialLabelsOnCard()
    {
        await NavigateToAsync("/people");
        await Page.GetByLabel("Name").FillAsync("Bob");
        await Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Add", Exact = true }).ClickAsync();

        var bobText = Page.GetByText("Bob");
        await bobText.First.WaitForAsync();

        await NavigateToAsync("/");

        var card = Page.Locator(".person-card");
        await card.WaitForAsync();

        var salaryText = Page.GetByText("Salary");
        await Assert.That(await salaryText.CountAsync()).IsGreaterThanOrEqualTo(1);

        var leftoverText = Page.GetByText("Leftover");
        await Assert.That(await leftoverText.CountAsync()).IsGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task Dashboard_ShowsMultiplePersonCards()
    {
        await NavigateToAsync("/people");

        await Page.GetByLabel("Name").FillAsync("Alice");
        await Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Add", Exact = true }).ClickAsync();
        var aliceText = Page.GetByText("Alice");
        await aliceText.First.WaitForAsync();

        await Page.GetByLabel("Name").FillAsync("Bob");
        await Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Add", Exact = true }).ClickAsync();
        var bobText = Page.GetByText("Bob");
        await bobText.First.WaitForAsync();

        await NavigateToAsync("/");

        var cards = Page.Locator(".person-card");
        await cards.First.WaitForAsync();
        await Assert.That(await cards.CountAsync()).IsEqualTo(2);
    }
}
