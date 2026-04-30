namespace FinanceSplit.Domain.ValueObjects;

/// <summary>
/// Discriminated union representing the three ways a recurrence can terminate.
/// The UI uses this to pre-select the correct option (never / count / date).
/// </summary>
public abstract record RecurrenceTermination
{
    // Private constructor prevents subclassing outside of the defined cases.
    private RecurrenceTermination() { }

    /// <summary>The recurrence runs indefinitely with no planned end.</summary>
    public sealed record Open : RecurrenceTermination;

    /// <summary>The recurrence runs for a fixed number of monthly occurrences.</summary>
    public sealed record ByCount(int Count) : RecurrenceTermination;

    /// <summary>The recurrence runs until and including a specific month.</summary>
    public sealed record ByDate(DateOnly EndMonth) : RecurrenceTermination;
}
