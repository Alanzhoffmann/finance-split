using FinanceSplit.Domain.Entities;
using FinanceSplit.Domain.ValueObjects;

namespace FinanceSplit.Domain.Services;

public class ExpenseCalculatorTests
{
    [Test]
    public async Task CalculateSettlements_EvenSplit_TwoPeople_ShouldReturnSingleSettlement()
    {
        var alice = new Person("Alice");
        var bob = new Person("Bob");
        var month = new DateTime(2026, 4, 10);

        var transactions = new[]
        {
            new Transaction(title: "Groceries", amount: 100m, paidBy: alice, splitPay: SplitPay.CreateEvenSplit([alice, bob]), date: month),
        };

        var calculator = new ExpenseCalculator();

        var settlements = calculator.CalculateSettlements(transactions);

        await Assert.That(settlements.Count).IsEqualTo(1);
        var settlement = settlements.Single();
        await Assert.That(settlement.From.Name).IsEqualTo("Bob");
        await Assert.That(settlement.To.Name).IsEqualTo("Alice");
        await Assert.That(settlement.Amount).IsEqualTo(50m);
    }

    [Test]
    public async Task CalculateSettlements_NoneSplit_ShouldReturnNoSettlements()
    {
        var alice = new Person("Alice");
        var transactions = new[]
        {
            new Transaction(title: "Personal", amount: 75m, paidBy: alice, splitPay: SplitPay.CreateNoneSplit(alice), date: new DateTime(2026, 4, 3)),
        };

        var calculator = new ExpenseCalculator();

        var settlements = calculator.CalculateSettlements(transactions);

        await Assert.That(settlements).IsEmpty();
    }

    [Test]
    public async Task CalculateSettlements_RatioSplit_ShouldUseMonthlySalaryWeights()
    {
        var alice = new Person("Alice");
        var bob = new Person("Bob");

        alice.AddSalary(new Salary(new DateOnly(2026, 4, 1), 9000m));
        bob.AddSalary(new Salary(new DateOnly(2026, 4, 1), 3000m));

        var transactions = new[]
        {
            new Transaction(title: "Rent", amount: 120m, paidBy: alice, splitPay: SplitPay.CreateRatioSplit([alice, bob]), date: new DateTime(2026, 4, 5)),
        };

        var calculator = new ExpenseCalculator();

        var settlements = calculator.CalculateSettlements(transactions);

        await Assert.That(settlements.Count).IsEqualTo(1);
        var settlement = settlements.Single();
        await Assert.That(settlement.From.Name).IsEqualTo("Bob");
        await Assert.That(settlement.To.Name).IsEqualTo("Alice");
        await Assert.That(settlement.Amount).IsEqualTo(30m);
    }

    [Test]
    public async Task BuildMonthlySummary_MixedTransactions_ShouldProduceExpectedSettlements()
    {
        var alice = new Person("Alice");
        var bob = new Person("Bob");
        var charlie = new Person("Charlie");

        alice.AddSalary(new Salary(new DateOnly(2026, 4, 1), 6000m));
        bob.AddSalary(new Salary(new DateOnly(2026, 4, 1), 3000m));
        charlie.AddSalary(new Salary(new DateOnly(2026, 4, 1), 1000m));

        var monthTransactions = new[]
        {
            new Transaction(
                title: "Dinner",
                amount: 100m,
                paidBy: alice,
                splitPay: SplitPay.CreateEvenSplit([alice, bob, charlie]),
                date: new DateTime(2026, 4, 5)
            ),
            new Transaction(
                title: "Utilities",
                amount: 90m,
                paidBy: bob,
                splitPay: SplitPay.CreateRatioSplit([alice, bob, charlie]),
                date: new DateTime(2026, 4, 20)
            ),
        };

        var calculator = new ExpenseCalculator();

        var settlements = calculator.CalculateSettlements(monthTransactions).OrderBy(s => s.From.Name).ThenBy(s => s.To.Name).ToArray();

        await Assert.That(settlements.Length).IsEqualTo(2);
        await Assert.That(settlements[0].From.Name).IsEqualTo("Charlie");
        await Assert.That(settlements[0].To.Name).IsEqualTo("Alice");
        await Assert.That(settlements[0].Amount).IsEqualTo(12.66m);

        await Assert.That(settlements[1].From.Name).IsEqualTo("Charlie");
        await Assert.That(settlements[1].To.Name).IsEqualTo("Bob");
        await Assert.That(settlements[1].Amount).IsEqualTo(29.67m);
    }

    [Test]
    public async Task BuildMonthlySummary_DifferentMonths_ShouldThrow()
    {
        var alice = new Person("Alice");
        var bob = new Person("Bob");

        var transactions = new[]
        {
            new Transaction("April", 20m, alice, SplitPay.CreateEvenSplit([alice, bob]), new DateTime(2026, 4, 29)),
            new Transaction("May", 20m, bob, SplitPay.CreateEvenSplit([alice, bob]), new DateTime(2026, 5, 1)),
        };

        var calculator = new ExpenseCalculator();

        var thrown = false;
        try
        {
            calculator.BuildMonthlySummary(transactions);
        }
        catch (InvalidOperationException)
        {
            thrown = true;
        }

        await Assert.That(thrown).IsTrue();
    }

    [Test]
    public async Task CalculateSettlements_RecurringForever_AppliesToAnyTargetMonth()
    {
        var alice = new Person("Alice");
        var bob = new Person("Bob");

        var rent = new Transaction(
            title: "Rent",
            amount: 1200m,
            paidBy: alice,
            splitPay: SplitPay.CreateEvenSplit([alice, bob]),
            date: new DateTime(2026, 1, 1),
            recurrence: Recurrence.Forever(new DateOnly(2026, 1, 1))
        );

        var calculator = new ExpenseCalculator();

        // Calculate for April even though the transaction started in January
        var settlements = calculator.CalculateSettlements(new DateOnly(2026, 4, 1), [rent]);

        await Assert.That(settlements.Count).IsEqualTo(1);
        var settlement = settlements.Single();
        await Assert.That(settlement.From.Name).IsEqualTo("Bob");
        await Assert.That(settlement.To.Name).IsEqualTo("Alice");
        await Assert.That(settlement.Amount).IsEqualTo(600m);
    }

    [Test]
    public async Task CalculateSettlements_RecurringFixedCount_ThrowsAfterExpiry()
    {
        var alice = new Person("Alice");
        var bob = new Person("Bob");

        // Runs for 3 months: Jan, Feb, Mar 2026
        var loan = new Transaction(
            title: "Loan repayment",
            amount: 300m,
            paidBy: alice,
            splitPay: SplitPay.CreateEvenSplit([alice, bob]),
            date: new DateTime(2026, 1, 1),
            recurrence: Recurrence.ForMonths(new DateOnly(2026, 1, 1), 3)
        );

        var calculator = new ExpenseCalculator();

        var thrown = false;
        try
        {
            calculator.BuildMonthlySummary(new DateOnly(2026, 4, 1), [loan]);
        }
        catch (InvalidOperationException)
        {
            thrown = true;
        }

        await Assert.That(thrown).IsTrue();
    }

    [Test]
    public async Task CalculateSettlements_MixedOneOffAndRecurring_CalculatesCorrectly()
    {
        var alice = new Person("Alice");
        var bob = new Person("Bob");

        // Recurring rent: started January, runs forever
        var rent = new Transaction(
            title: "Rent",
            amount: 800m,
            paidBy: alice,
            splitPay: SplitPay.CreateEvenSplit([alice, bob]),
            date: new DateTime(2026, 1, 1),
            recurrence: Recurrence.Forever(new DateOnly(2026, 1, 1))
        );

        // One-off dinner in April
        var dinner = new Transaction(
            title: "Dinner",
            amount: 60m,
            paidBy: bob,
            splitPay: SplitPay.CreateEvenSplit([alice, bob]),
            date: new DateTime(2026, 4, 15)
        );

        var calculator = new ExpenseCalculator();

        var settlements = calculator.CalculateSettlements(new DateOnly(2026, 4, 1), [rent, dinner]);

        // Alice paid 800 rent, owes 30 from dinner → net +770 credit
        // Bob paid 60 dinner, owes 400 from rent → net -340 debit
        // After netting: Bob owes Alice 370
        await Assert.That(settlements.Count).IsEqualTo(1);
        var settlement = settlements.Single();
        await Assert.That(settlement.From.Name).IsEqualTo("Bob");
        await Assert.That(settlement.To.Name).IsEqualTo("Alice");
        await Assert.That(settlement.Amount).IsEqualTo(370m);
    }

    [Test]
    public async Task BuildMonthlySummary_RecurringFixedCount_LastMonthIsValid()
    {
        var alice = new Person("Alice");
        var bob = new Person("Bob");

        var subscription = new Transaction(
            title: "Subscription",
            amount: 20m,
            paidBy: alice,
            splitPay: SplitPay.CreateEvenSplit([alice, bob]),
            date: new DateTime(2026, 1, 1),
            recurrence: Recurrence.ForMonths(new DateOnly(2026, 1, 1), 3)
        );

        var calculator = new ExpenseCalculator();

        // March is the last valid month (3rd occurrence)
        var settlements = calculator.CalculateSettlements(new DateOnly(2026, 3, 1), [subscription]);

        await Assert.That(settlements.Count).IsEqualTo(1);
        await Assert.That(settlements.Single().Amount).IsEqualTo(10m);
    }
}
