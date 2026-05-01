namespace FinanceSplit.Domain.ValueObjects;

/// <summary>
/// Describes how a transaction repeats monthly. <see cref="Termination"/> is the canonical
/// field; <see cref="EndMonth"/> and <see cref="OccurrenceCount"/> are derived from it.
/// Use <see cref="ClosedAt"/> to cap any recurrence with a specific end date at a later time.
/// </summary>
public record Recurrence
{
    // Required for EF Core materialization
    private Recurrence() { }

    private Recurrence(DateOnly startMonth, RecurrenceTermination termination)
    {
        StartMonth = new DateOnly(startMonth.Year, startMonth.Month, 1);
        Termination = NormaliseAndValidate(StartMonth, termination);
    }

    public DateOnly StartMonth { get; init; }

    /// <summary>
    /// The termination mode. Match on this to determine which UI option to show:
    /// <see cref="RecurrenceTermination.Open"/> → never,
    /// <see cref="RecurrenceTermination.ByCount"/> → count,
    /// <see cref="RecurrenceTermination.ByDate"/> → date.
    /// </summary>
    public RecurrenceTermination Termination { get; init; } = null!;

    public bool IsForever => Termination is RecurrenceTermination.Open;

    /// <summary>The last inclusive month of the recurrence. Null when forever.</summary>
    public DateOnly? EndMonth =>
        Termination switch
        {
            RecurrenceTermination.Open => null,
            RecurrenceTermination.ByCount(var count) => StartMonth.AddMonths(count - 1),
            RecurrenceTermination.ByDate(var end) => end,
            _ => throw new InvalidOperationException($"Unsupported termination: {Termination}"),
        };

    /// <summary>Number of monthly occurrences. Null when forever.</summary>
    public int? OccurrenceCount =>
        Termination switch
        {
            RecurrenceTermination.Open => null,
            RecurrenceTermination.ByCount(var count) => count,
            RecurrenceTermination.ByDate(var end) => ((end.Year - StartMonth.Year) * 12) + end.Month - StartMonth.Month + 1,
            _ => throw new InvalidOperationException($"Unsupported termination: {Termination}"),
        };

    public bool OccursIn(DateOnly month)
    {
        var normalised = new DateOnly(month.Year, month.Month, 1);

        if (normalised < StartMonth)
        {
            return false;
        }

        return !EndMonth.HasValue || normalised <= EndMonth.Value;
    }

    /// <summary>
    /// Returns a new recurrence with the same start but closed at <paramref name="endMonth"/>.
    /// The resulting termination is always <see cref="RecurrenceTermination.ByDate"/>,
    /// regardless of the original mode.
    /// </summary>
    public Recurrence ClosedAt(DateOnly endMonth) => new(StartMonth, new RecurrenceTermination.ByDate(endMonth));

    public static Recurrence Forever(DateOnly startMonth) => new(startMonth, new RecurrenceTermination.Open());

    public static Recurrence ForMonths(DateOnly startMonth, int count)
    {
        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Occurrence count must be positive.");
        }

        return new(startMonth, new RecurrenceTermination.ByCount(count));
    }

    public static Recurrence UntilMonth(DateOnly startMonth, DateOnly endMonth) => new(startMonth, new RecurrenceTermination.ByDate(endMonth));

    private static RecurrenceTermination NormaliseAndValidate(DateOnly start, RecurrenceTermination termination)
    {
        if (termination is RecurrenceTermination.ByDate(var end))
        {
            var normalisedEnd = new DateOnly(end.Year, end.Month, 1);

            if (normalisedEnd < start)
            {
                throw new ArgumentException("End month cannot be before start month.", "endMonth");
            }

            return new RecurrenceTermination.ByDate(normalisedEnd);
        }

        return termination;
    }
}
