using TUnit.Core;

namespace FinanceSplit.Web.Tests.Pages;

public class PeoplePageTests : WebPageTest
{
    [Test]
    public async Task PeoplePage_DisplaysHeading()
    {
        await NavigateToAsync("/people");

        var heading = Page.GetByText("People").First;
        await heading.WaitForAsync();
        await Assert.That(await heading.CountAsync()).IsGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task PeoplePage_HasAddPersonSection()
    {
        await NavigateToAsync("/people");

        var addPersonText = Page.GetByText("Add Person");
        await Assert.That(await addPersonText.CountAsync()).IsGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task PeoplePage_CanAddPerson()
    {
        await NavigateToAsync("/people");

        var nameInput = Page.GetByLabel("Name");
        await nameInput.FillAsync("Alice");

        await Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Add", Exact = true }).ClickAsync();

        var aliceText = Page.GetByText("Alice");
        await aliceText.First.WaitForAsync();
        await Assert.That(await aliceText.CountAsync()).IsGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task PeoplePage_ShowsTable_WhenPeopleExist()
    {
        await NavigateToAsync("/people");

        var nameInput = Page.GetByLabel("Name");
        await nameInput.FillAsync("Bob");
        await Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Add", Exact = true }).ClickAsync();

        var bobText = Page.GetByText("Bob");
        await bobText.First.WaitForAsync();
        await Assert.That(await bobText.CountAsync()).IsGreaterThanOrEqualTo(1);
    }
}
