using Bunit;
using FinanceSplit.Web.Components.Pages;
using TUnit.Core;

namespace FinanceSplit.Web.ComponentTests.Pages;

public class PeopleComponentTests : ComponentTestBase
{
    [Test]
    public async Task PeoplePage_RendersHeading()
    {
        var cut = Ctx.Render<People>();

        var heading = cut.Find("h1");
        await Assert.That(heading.TextContent).IsEqualTo("People");
    }

    [Test]
    public async Task PeoplePage_RendersAddForm()
    {
        var cut = Ctx.Render<People>();

        var input = cut.Find("input[placeholder='Name']");
        await Assert.That(input).IsNotNull();

        var button = cut.Find("button[type='submit']");
        await Assert.That(button.TextContent).IsEqualTo("Add");
    }

    [Test]
    public async Task PeoplePage_NoTable_WhenNoPeople()
    {
        var cut = Ctx.Render<People>();

        var tables = cut.FindAll("table");
        await Assert.That(tables.Count).IsEqualTo(0);
    }

    [Test]
    public async Task PeoplePage_AddsPersonAndShowsTable()
    {
        var cut = Ctx.Render<People>();

        var input = cut.Find("input[placeholder='Name']");
        input.Change("Alice");

        var form = cut.Find("form");
        await form.SubmitAsync();

        var cells = cut.FindAll("td");
        var aliceCell = cells.FirstOrDefault(c => c.TextContent == "Alice");
        await Assert.That(aliceCell).IsNotNull();
    }

    [Test]
    public async Task PeoplePage_ShowsTableHeaders()
    {
        var cut = Ctx.Render<People>();

        // Add a person to make the table appear
        cut.Find("input[placeholder='Name']").Change("Bob");
        await cut.Find("form").SubmitAsync();

        var headers = cut.FindAll("th");
        await Assert.That(headers.Count).IsEqualTo(3);
        await Assert.That(headers[0].TextContent).IsEqualTo("Name");
        await Assert.That(headers[1].TextContent).IsEqualTo("Salaries");
        await Assert.That(headers[2].TextContent).IsEqualTo("Actions");
    }

    [Test]
    public async Task PeoplePage_ShowsAddSalaryButton_AfterPersonAdded()
    {
        var cut = Ctx.Render<People>();

        cut.Find("input[placeholder='Name']").Change("Charlie");
        await cut.Find("form").SubmitAsync();

        var salaryButtons = cut.FindAll("button").Where(b => b.TextContent == "Add Salary").ToList();
        await Assert.That(salaryButtons.Count).IsEqualTo(1);
    }
}
