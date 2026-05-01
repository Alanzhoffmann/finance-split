using TUnit.Core;

namespace FinanceSplit.Web.Tests.Pages;

public class PeoplePageTests : WebPageTest
{
    [Test]
    public async Task PeoplePage_DisplaysHeading()
    {
        await NavigateToAsync("/people");

        var heading = Page.GetByRole(Microsoft.Playwright.AriaRole.Heading, new() { Name = "People" });
        await Assert.That(await heading.CountAsync()).IsGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task PeoplePage_HasAddPersonForm()
    {
        await NavigateToAsync("/people");

        var nameInput = Page.GetByPlaceholder("Name");
        await Assert.That(await nameInput.CountAsync()).IsEqualTo(1);

        var addButton = Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Add" });
        await Assert.That(await addButton.CountAsync()).IsGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task PeoplePage_CanAddPerson()
    {
        await NavigateToAsync("/people");

        await Page.GetByPlaceholder("Name").FillAsync("Alice");
        await Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Add" }).ClickAsync();

        var cell = Page.GetByRole(Microsoft.Playwright.AriaRole.Cell, new() { Name = "Alice" });
        await cell.WaitForAsync();
        await Assert.That(await cell.CountAsync()).IsEqualTo(1);
    }

    [Test]
    public async Task PeoplePage_ShowsPeopleTable_WhenPeopleExist()
    {
        await NavigateToAsync("/people");

        await Page.GetByPlaceholder("Name").FillAsync("Bob");
        await Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Add" }).ClickAsync();

        var table = Page.Locator("table");
        await table.WaitForAsync();
        await Assert.That(await table.CountAsync()).IsEqualTo(1);

        var nameHeader = Page.Locator("th", new() { HasTextString = "Name" });
        await Assert.That(await nameHeader.CountAsync()).IsEqualTo(1);
    }

    [Test]
    public async Task PeoplePage_HasAddSalaryForm_AfterPersonAdded()
    {
        await NavigateToAsync("/people");

        await Page.GetByPlaceholder("Name").FillAsync("Charlie");
        await Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Add" }).ClickAsync();

        var salaryButton = Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Add Salary" });
        await salaryButton.WaitForAsync();
        await Assert.That(await salaryButton.CountAsync()).IsGreaterThanOrEqualTo(1);
    }
}
