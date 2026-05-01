using FinanceSplit.Domain.Entities;
using FinanceSplit.Domain.ValueObjects;

namespace FinanceSplit.Domain.Tests.Entities;

public class PersonTests
{
    [Test]
    public async Task GetSalaryForMonth_NoSalary_ShouldReturnZero()
    {
        var person = new Person("Alice");

        var salary = person.GetSalaryForMonth(new DateOnly(2026, 4, 1));

        await Assert.That(salary).IsEqualTo(0m);
    }

    [Test]
    public async Task GetSalaryForMonth_SalaryAddedInSameMonth_ShouldReturnThatSalary()
    {
        var person = new Person("Alice");
        person.AddSalary(new Salary(new DateOnly(2026, 4, 1), 5000m));

        var salary = person.GetSalaryForMonth(new DateOnly(2026, 4, 1));

        await Assert.That(salary).IsEqualTo(5000m);
    }

    [Test]
    public async Task GetSalaryForMonth_SalaryAddedMidMonth_ShouldStillApplyToThatMonth()
    {
        // A salary dated April 15 normalises to April 1 and therefore covers April expenses.
        var person = new Person("Alice");
        person.AddSalary(new Salary(new DateOnly(2026, 4, 15), 5000m));

        var salary = person.GetSalaryForMonth(new DateOnly(2026, 4, 1));

        await Assert.That(salary).IsEqualTo(5000m);
    }

    [Test]
    public async Task GetSalaryForMonth_PreviousMonthSalary_ShouldCarryForwardToLaterMonths()
    {
        var person = new Person("Alice");
        person.AddSalary(Salary.ForMonth(2026, 1, 4000m));

        await Assert.That(person.GetSalaryForMonth(new DateOnly(2026, 4, 1))).IsEqualTo(4000m);
        await Assert.That(person.GetSalaryForMonth(new DateOnly(2026, 12, 1))).IsEqualTo(4000m);
    }

    [Test]
    public async Task GetSalaryForMonth_SalaryInFutureMonth_ShouldNotApplyYet()
    {
        // A May salary must not influence April expenses.
        var person = new Person("Alice");
        person.AddSalary(Salary.ForMonth(2026, 5, 6000m));

        var salary = person.GetSalaryForMonth(new DateOnly(2026, 4, 1));

        await Assert.That(salary).IsEqualTo(0m);
    }

    [Test]
    public async Task GetSalaryForMonth_RaiseMidYear_ShouldUseOldSalaryBeforeAndNewSalaryAfter()
    {
        var person = new Person("Alice");
        person.AddSalary(Salary.ForMonth(2026, 1, 4000m));
        person.AddSalary(Salary.ForMonth(2026, 4, 5500m));

        await Assert.That(person.GetSalaryForMonth(new DateOnly(2026, 3, 1))).IsEqualTo(4000m);
        await Assert.That(person.GetSalaryForMonth(new DateOnly(2026, 4, 1))).IsEqualTo(5500m);
        await Assert.That(person.GetSalaryForMonth(new DateOnly(2026, 6, 1))).IsEqualTo(5500m);
    }

    [Test]
    public async Task Salary_NormalisesDateToFirstOfMonth()
    {
        var salary = new Salary(new DateOnly(2026, 4, 17), 3000m);

        await Assert.That(salary.Date).IsEqualTo(new DateOnly(2026, 4, 1));
    }

    [Test]
    public async Task Salary_ForMonth_CreatesNormalisedSalary()
    {
        var salary = Salary.ForMonth(2026, 4, 3000m);

        await Assert.That(salary.Date).IsEqualTo(new DateOnly(2026, 4, 1));
        await Assert.That(salary.Amount).IsEqualTo(3000m);
    }

    [Test]
    public async Task GetSalaryForMonth_FutureSalaryExists_StillReturnsZeroForActual()
    {
        // Confirms actual salary is unaffected by planned future salaries.
        var person = new Person("Alice");
        person.AddSalary(Salary.ForMonth(2026, 7, 6000m));

        await Assert.That(person.GetSalaryForMonth(new DateOnly(2026, 4, 1))).IsEqualTo(0m);
    }
}
