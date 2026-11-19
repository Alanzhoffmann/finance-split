using FinanceSplit.Contracts.People.DTOs;
using FinanceSplit.Contracts.Transactions.Enums;

namespace FinanceSplit.Contracts.Transactions.DTOs;

public record TransactionDto(
    Guid Id,
    string Name,
    decimal Value,
    DateOnly Month,
    SplitType SplitType,
    PersonDto PaidBy,
    List<TransactionShareDto> SharedWith,
    RecurrenceDto? Recurrence = null
);

public record TransactionShareDto(PersonDto Person, decimal Amount);

public record RecurrenceDto(
    Guid RecurrenceId,
    RecurrenceType Type,
    int? NumberOfRecurrences,
    DateOnly? LastMonth
);
