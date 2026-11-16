using FinanceSplit.Contracts.Transactions.Enums;

namespace FinanceSplit.Contracts.Transactions.Commands;

public record CreateTransactionCommand(
    string Name,
    DateOnly Date,
    decimal Value,
    Guid PaidByPersonId,
    SplitType SplitType,
    IReadOnlyCollection<Guid> SharedWithIds,
    SplitTransactionSetup? SplitSetup = null,
    RecurrenceTransactionSetup? RecurrenceSetup = null
);

public record SplitTransactionSetup(int NumberOfInstallments);

public record RecurrenceTransactionSetup(
    int? NumberOfRecurrences = null,
    DateOnly? LastMonth = null
)
{
    public RecurrenceType Type =>
        NumberOfRecurrences is not null ? RecurrenceType.NumberOfTimes
        : LastMonth is not null ? RecurrenceType.PeriodOfTime
        : RecurrenceType.Forever;
}
