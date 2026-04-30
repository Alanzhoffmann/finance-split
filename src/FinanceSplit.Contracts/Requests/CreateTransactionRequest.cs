using FinanceSplit.Contracts.Enums;

namespace FinanceSplit.Contracts.Requests;

public record CreateTransactionRequest(
    string Title,
    decimal Amount,
    Guid PaidById,
    SplitType SplitType,
    List<Guid> ParticipantIds,
    DateTime? Date = null,
    CreateRecurrenceRequest? Recurrence = null
);

public record CreateRecurrenceRequest(DateOnly StartMonth, RecurrenceTerminationType TerminationType, int? OccurrenceCount = null, DateOnly? EndMonth = null);
