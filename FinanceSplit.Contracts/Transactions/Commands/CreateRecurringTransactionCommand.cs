using FinanceSplit.Contracts.Transactions.Enums;

namespace FinanceSplit.Contracts.Transactions.Commands;

public record CreateRecurringTransactionCommand(
    string Name,
    DateOnly Date,
    decimal Value,
    RecurrenceType Type,
    int? NumberOfRecurrences,
    DateOnly? LastMonth,
    Guid PaidByPersonId,
    SplitType SplitType,
    IReadOnlyCollection<Guid> SharedWithIds
);
