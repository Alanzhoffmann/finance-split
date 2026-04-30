namespace FinanceSplit.Domain.ValueObjects;

public class RecurrenceTests
{
    [Test]
    public async Task OccursIn_BeforeStartMonth_ReturnsFalse()
    {
        var recurrence = Recurrence.Forever(new DateOnly(2026, 4, 1));

        await Assert.That(recurrence.OccursIn(new DateOnly(2026, 3, 1))).IsFalse();
    }

    [Test]
    public async Task OccursIn_StartMonth_ReturnsTrue()
    {
        var recurrence = Recurrence.Forever(new DateOnly(2026, 4, 1));

        await Assert.That(recurrence.OccursIn(new DateOnly(2026, 4, 1))).IsTrue();
    }

    [Test]
    public async Task OccursIn_Forever_ReturnsTrueForAnyMonthAfterStart()
    {
        var recurrence = Recurrence.Forever(new DateOnly(2026, 1, 1));

        await Assert.That(recurrence.OccursIn(new DateOnly(2026, 6, 1))).IsTrue();
        await Assert.That(recurrence.OccursIn(new DateOnly(2030, 1, 1))).IsTrue();
    }

    [Test]
    public async Task OccursIn_FixedCount_ReturnsTrueWithinRange()
    {
        // 3 occurrences: Jan, Feb, Mar 2026
        var recurrence = Recurrence.ForMonths(new DateOnly(2026, 1, 1), 3);

        await Assert.That(recurrence.OccursIn(new DateOnly(2026, 1, 1))).IsTrue();
        await Assert.That(recurrence.OccursIn(new DateOnly(2026, 2, 1))).IsTrue();
        await Assert.That(recurrence.OccursIn(new DateOnly(2026, 3, 1))).IsTrue();
    }

    [Test]
    public async Task OccursIn_FixedCount_LastMonth_ReturnsTrue()
    {
        var recurrence = Recurrence.ForMonths(new DateOnly(2026, 1, 1), 3);

        await Assert.That(recurrence.OccursIn(new DateOnly(2026, 3, 1))).IsTrue();
    }

    [Test]
    public async Task OccursIn_FixedCount_MonthAfterLast_ReturnsFalse()
    {
        var recurrence = Recurrence.ForMonths(new DateOnly(2026, 1, 1), 3);

        await Assert.That(recurrence.OccursIn(new DateOnly(2026, 4, 1))).IsFalse();
    }

    [Test]
    public async Task EndMonth_Forever_IsNull()
    {
        var recurrence = Recurrence.Forever(new DateOnly(2026, 1, 1));

        await Assert.That(recurrence.EndMonth).IsNull();
    }

    [Test]
    public async Task EndMonth_FixedCount_IsCorrect()
    {
        var recurrence = Recurrence.ForMonths(new DateOnly(2026, 1, 1), 3);

        await Assert.That(recurrence.EndMonth).IsEqualTo(new DateOnly(2026, 3, 1));
    }

    [Test]
    public async Task StartMonth_NormalisesToFirstOfMonth()
    {
        var recurrence = Recurrence.Forever(new DateOnly(2026, 4, 17));

        await Assert.That(recurrence.StartMonth).IsEqualTo(new DateOnly(2026, 4, 1));
    }

    [Test]
    public async Task ForMonths_ZeroCount_Throws()
    {
        var thrown = false;
        try
        {
            Recurrence.ForMonths(new DateOnly(2026, 1, 1), 0);
        }
        catch (ArgumentOutOfRangeException)
        {
            thrown = true;
        }

        await Assert.That(thrown).IsTrue();
    }

    // --- UntilMonth ---

    [Test]
    public async Task UntilMonth_EndMonth_IsNormalised()
    {
        var recurrence = Recurrence.UntilMonth(new DateOnly(2026, 1, 1), new DateOnly(2026, 6, 15));

        await Assert.That(recurrence.EndMonth).IsEqualTo(new DateOnly(2026, 6, 1));
    }

    [Test]
    public async Task UntilMonth_OccursIn_EndMonth_ReturnsTrue()
    {
        var recurrence = Recurrence.UntilMonth(new DateOnly(2026, 1, 1), new DateOnly(2026, 6, 1));

        await Assert.That(recurrence.OccursIn(new DateOnly(2026, 6, 1))).IsTrue();
    }

    [Test]
    public async Task UntilMonth_OccursIn_MonthAfterEnd_ReturnsFalse()
    {
        var recurrence = Recurrence.UntilMonth(new DateOnly(2026, 1, 1), new DateOnly(2026, 6, 1));

        await Assert.That(recurrence.OccursIn(new DateOnly(2026, 7, 1))).IsFalse();
    }

    [Test]
    public async Task UntilMonth_OccurrenceCount_IsCorrect()
    {
        // Jan to Jun = 6 months
        var recurrence = Recurrence.UntilMonth(new DateOnly(2026, 1, 1), new DateOnly(2026, 6, 1));

        await Assert.That(recurrence.OccurrenceCount).IsEqualTo(6);
    }

    [Test]
    public async Task UntilMonth_EndBeforeStart_Throws()
    {
        var thrown = false;
        try
        {
            Recurrence.UntilMonth(new DateOnly(2026, 6, 1), new DateOnly(2026, 1, 1));
        }
        catch (ArgumentException)
        {
            thrown = true;
        }

        await Assert.That(thrown).IsTrue();
    }

    [Test]
    public async Task UntilMonth_SameMonthAsStart_IsOneOccurrence()
    {
        var recurrence = Recurrence.UntilMonth(new DateOnly(2026, 4, 1), new DateOnly(2026, 4, 1));

        await Assert.That(recurrence.OccurrenceCount).IsEqualTo(1);
        await Assert.That(recurrence.OccursIn(new DateOnly(2026, 4, 1))).IsTrue();
        await Assert.That(recurrence.OccursIn(new DateOnly(2026, 5, 1))).IsFalse();
    }

    // --- ClosedAt ---

    [Test]
    public async Task ClosedAt_Forever_BecomesBounded()
    {
        var recurrence = Recurrence.Forever(new DateOnly(2026, 1, 1)).ClosedAt(new DateOnly(2026, 6, 1));

        await Assert.That(recurrence.IsForever).IsFalse();
        await Assert.That(recurrence.EndMonth).IsEqualTo(new DateOnly(2026, 6, 1));
    }

    [Test]
    public async Task ClosedAt_DoesNotMutateOriginal()
    {
        var original = Recurrence.Forever(new DateOnly(2026, 1, 1));
        var closed = original.ClosedAt(new DateOnly(2026, 6, 1));

        await Assert.That(original.IsForever).IsTrue();
        await Assert.That(closed.IsForever).IsFalse();
    }

    [Test]
    public async Task ClosedAt_EndMonth_NormalisedToFirstOfMonth()
    {
        var recurrence = Recurrence.Forever(new DateOnly(2026, 1, 1)).ClosedAt(new DateOnly(2026, 6, 20));

        await Assert.That(recurrence.EndMonth).IsEqualTo(new DateOnly(2026, 6, 1));
    }

    [Test]
    public async Task ClosedAt_OccursIn_AfterEnd_ReturnsFalse()
    {
        var recurrence = Recurrence.Forever(new DateOnly(2026, 1, 1)).ClosedAt(new DateOnly(2026, 6, 1));

        await Assert.That(recurrence.OccursIn(new DateOnly(2026, 7, 1))).IsFalse();
    }

    [Test]
    public async Task ClosedAt_BeforeStart_Throws()
    {
        var thrown = false;
        try
        {
            Recurrence.Forever(new DateOnly(2026, 6, 1)).ClosedAt(new DateOnly(2026, 1, 1));
        }
        catch (ArgumentException)
        {
            thrown = true;
        }

        await Assert.That(thrown).IsTrue();
    }

    // --- OccurrenceCount derivation ---

    [Test]
    public async Task ForMonths_OccurrenceCount_RoundTrips()
    {
        var recurrence = Recurrence.ForMonths(new DateOnly(2026, 1, 1), 12);

        await Assert.That(recurrence.OccurrenceCount).IsEqualTo(12);
    }

    [Test]
    public async Task Forever_OccurrenceCount_IsNull()
    {
        var recurrence = Recurrence.Forever(new DateOnly(2026, 1, 1));

        await Assert.That(recurrence.OccurrenceCount).IsNull();
    }

    // --- Termination type ---

    [Test]
    public async Task Forever_Termination_IsOpen()
    {
        var recurrence = Recurrence.Forever(new DateOnly(2026, 1, 1));

        await Assert.That(recurrence.Termination is RecurrenceTermination.Open).IsTrue();
    }

    [Test]
    public async Task ForMonths_Termination_IsByCount()
    {
        var recurrence = Recurrence.ForMonths(new DateOnly(2026, 1, 1), 6);

        await Assert.That(recurrence.Termination is RecurrenceTermination.ByCount).IsTrue();
    }

    [Test]
    public async Task ForMonths_Termination_ByCount_CarriesCount()
    {
        var recurrence = Recurrence.ForMonths(new DateOnly(2026, 1, 1), 6);
        var byCount = recurrence.Termination as RecurrenceTermination.ByCount;

        await Assert.That(byCount).IsNotNull();
        await Assert.That(byCount!.Count).IsEqualTo(6);
    }

    [Test]
    public async Task UntilMonth_Termination_IsByDate()
    {
        var recurrence = Recurrence.UntilMonth(new DateOnly(2026, 1, 1), new DateOnly(2026, 6, 1));

        await Assert.That(recurrence.Termination is RecurrenceTermination.ByDate).IsTrue();
    }

    [Test]
    public async Task UntilMonth_Termination_ByDate_CarriesNormalisedEndMonth()
    {
        var recurrence = Recurrence.UntilMonth(new DateOnly(2026, 1, 1), new DateOnly(2026, 6, 20));
        var byDate = recurrence.Termination as RecurrenceTermination.ByDate;

        await Assert.That(byDate).IsNotNull();
        await Assert.That(byDate!.EndMonth).IsEqualTo(new DateOnly(2026, 6, 1));
    }

    [Test]
    public async Task ClosedAt_Termination_IsByDate_RegardlessOfOriginalMode()
    {
        var fromForever = Recurrence.Forever(new DateOnly(2026, 1, 1)).ClosedAt(new DateOnly(2026, 6, 1));
        var fromByCount = Recurrence.ForMonths(new DateOnly(2026, 1, 1), 12).ClosedAt(new DateOnly(2026, 6, 1));

        await Assert.That(fromForever.Termination is RecurrenceTermination.ByDate).IsTrue();
        await Assert.That(fromByCount.Termination is RecurrenceTermination.ByDate).IsTrue();
    }

    [Test]
    public async Task ClosedAt_DoesNotMutateOriginalTermination()
    {
        var original = Recurrence.Forever(new DateOnly(2026, 1, 1));
        _ = original.ClosedAt(new DateOnly(2026, 6, 1));

        await Assert.That(original.Termination is RecurrenceTermination.Open).IsTrue();
    }

    [Test]
    public async Task ByCount_OccurrenceCount_MatchesStoredCount_NotDerivedFromDate()
    {
        // ForMonths(start, 6) and UntilMonth(start, start+5months) produce the same
        // EndMonth, but their Termination types are different and OccurrenceCount for
        // ByCount comes from the stored int, not from date arithmetic.
        var byCount = Recurrence.ForMonths(new DateOnly(2026, 1, 1), 6);
        var byDate = Recurrence.UntilMonth(new DateOnly(2026, 1, 1), new DateOnly(2026, 6, 1));

        await Assert.That(byCount.EndMonth).IsEqualTo(byDate.EndMonth);
        await Assert.That(byCount.OccurrenceCount).IsEqualTo(byDate.OccurrenceCount);
        await Assert.That(byCount.Termination is RecurrenceTermination.ByCount).IsTrue();
        await Assert.That(byDate.Termination is RecurrenceTermination.ByDate).IsTrue();
    }
}
