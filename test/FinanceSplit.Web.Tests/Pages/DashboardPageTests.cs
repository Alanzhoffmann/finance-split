using TUnit.Core;

namespace FinanceSplit.Web.Tests.Pages;

public class DashboardPageTests : WebPageTest
{
    [Test]
    public async Task Dashboard_DisplaysHeading()
    {
        await NavigateToAsync("/");

        var heading = Page.GetByRole(Microsoft.Playwright.AriaRole.Heading, new() { Name = "Dashboard" });
        await Assert.That(await heading.CountAsync()).IsEqualTo(1);
    }

    [Test]
    public async Task Dashboard_ShowsEmptyState_WhenNoPeople()
    {
        await NavigateToAsync("/");

        var message = Page.GetByText("No people added yet.");
        await Assert.That(await message.CountAsync()).IsEqualTo(1);
    }

    [Test]
    public async Task Dashboard_HasMonthSelector()
    {
        await NavigateToAsync("/");

        var monthInput = Page.Locator("input[type='month']");
        await Assert.That(await monthInput.CountAsync()).IsEqualTo(1);
    }

    [Test]
    public async Task Dashboard_ShowsPersonCard_AfterPersonAdded()
    {
        // Add a person via the People page
        await NavigateToAsync("/people");
        await Page.GetByPlaceholder("Name").FillAsync("Alice");
        await Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Add" }).ClickAsync();
        var cell = Page.GetByRole(Microsoft.Playwright.AriaRole.Cell, new() { Name = "Alice" });
        await cell.WaitForAsync();

        // Navigate to dashboard
        await NavigateToAsync("/");

        var card = Page.Locator(".person-card");
        await card.WaitForAsync();
        await Assert.That(await card.CountAsync()).IsEqualTo(1);

        var personName = Page.Locator(".person-card h2", new() { HasTextString = "Alice" });
        await Assert.That(await personName.CountAsync()).IsEqualTo(1);
    }

    [Test]
    public async Task Dashboard_ShowsSalaryOnCard()
    {
        await NavigateToAsync("/");

        var salaryLabel = Page.Locator("dt", new() { HasTextString = "Salary" });
        await Assert.That(await salaryLabel.CountAsync()).IsEqualTo(0);

        // Add a person
        await NavigateToAsync("/people");
        await Page.GetByPlaceholder("Name").FillAsync("Bob");
        await Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Add" }).ClickAsync();
        var cell = Page.GetByRole(Microsoft.Playwright.AriaRole.Cell, new() { Name = "Bob" });
        await cell.WaitForAsync();

        // Navigate to dashboard and verify card has salary fields
        await NavigateToAsync("/");

        var card = Page.Locator(".person-card");
        await card.WaitForAsync();

        var salaryDt = card.Locator("dt", new() { HasTextString = "Salary" });
        await Assert.That(await salaryDt.CountAsync()).IsEqualTo(1);

        var personalDt = card.Locator("dt", new() { HasTextString = "Personal Expenses" });
        await Assert.That(await personalDt.CountAsync()).IsEqualTo(1);

        var sharedDt = card.Locator("dt", new() { HasTextString = "Shared Expenses" });
        await Assert.That(await sharedDt.CountAsync()).IsEqualTo(1);

        var leftoverDt = card.Locator("dt", new() { HasTextString = "Leftover" });
        await Assert.That(await leftoverDt.CountAsync()).IsEqualTo(1);
    }

    [Test]
    public async Task Dashboard_ShowsMultiplePersonCards()
    {
        // Add two people
        await NavigateToAsync("/people");

        await Page.GetByPlaceholder("Name").FillAsync("Alice");
        await Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Add" }).ClickAsync();
        var aliceCell = Page.GetByRole(Microsoft.Playwright.AriaRole.Cell, new() { Name = "Alice" });
        await aliceCell.WaitForAsync();

        await Page.GetByPlaceholder("Name").FillAsync("Bob");
        await Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Add" }).ClickAsync();
        var bobCell = Page.GetByRole(Microsoft.Playwright.AriaRole.Cell, new() { Name = "Bob" });
        await bobCell.WaitForAsync();

        // Navigate to dashboard
        await NavigateToAsync("/");

        var cards = Page.Locator(".person-card");
        await cards.First.WaitForAsync();
        await Assert.That(await cards.CountAsync()).IsEqualTo(2);
    }
}
