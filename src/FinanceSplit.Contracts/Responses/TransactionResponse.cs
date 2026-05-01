using FinanceSplit.Contracts.Enums;

namespace FinanceSplit.Contracts.Responses;

public record TransactionResponse(
    Guid Id,
    string Title,
    string Description,
    decimal Amount,
    DateTime Date,
    PersonResponse PaidBy,
    SplitType SplitType,
    IReadOnlyCollection<PersonResponse> SplitParticipants,
    RecurrenceResponse? Recurrence
);
