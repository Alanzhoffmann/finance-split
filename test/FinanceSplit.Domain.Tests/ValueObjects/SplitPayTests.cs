using FinanceSplit.Contracts.Enums;
using FinanceSplit.Domain.Entities;

namespace FinanceSplit.Domain.ValueObjects;

public class SplitPayTests
{
    [Test]
    public async Task CreateNoneSplit_ShouldCreateNoneSplit()
    {
        // Arrange
        var person = new Person("John");

        // Act
        var splitPay = SplitPay.CreateNoneSplit(person);

        // Assert
        await Assert
            .That(splitPay)
            .IsNotNull()
            .And.Member(s => s.People, p => p.IsNotNull().And.IsSingleElement().And.Contains(person))
            .And.Member(s => s.SplitType, st => st.IsEqualTo(SplitType.None));
    }

    [Test]
    public async Task CalculateShares_EvenSplit_ShouldDistributeAmountByCents()
    {
        var alice = new Person("Alice");
        var bob = new Person("Bob");
        var charlie = new Person("Charlie");

        var splitPay = SplitPay.CreateEvenSplit([alice, bob, charlie]);

        var shares = splitPay.CalculateShares(100m, new DateOnly(2026, 4, 1));

        await Assert.That(shares[alice]).IsEqualTo(33.34m);
        await Assert.That(shares[bob]).IsEqualTo(33.33m);
        await Assert.That(shares[charlie]).IsEqualTo(33.33m);
    }

    [Test]
    public async Task CalculateShares_RatioSplit_ShouldUseMonthlySalaries()
    {
        var alice = new Person("Alice");
        var bob = new Person("Bob");

        alice.AddSalary(new Salary(new DateOnly(2026, 1, 1), 9000m));
        bob.AddSalary(new Salary(new DateOnly(2026, 1, 1), 3000m));

        var splitPay = SplitPay.CreateRatioSplit([alice, bob]);

        var shares = splitPay.CalculateShares(120m, new DateOnly(2026, 4, 1));

        await Assert.That(shares[alice]).IsEqualTo(90m);
        await Assert.That(shares[bob]).IsEqualTo(30m);
    }
}
