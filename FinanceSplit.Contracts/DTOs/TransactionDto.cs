using FinanceSplit.Contracts.Enums;

namespace FinanceSplit.Contracts.DTOs;

public record TransactionDto(
    Guid Id,
    string Name,
    decimal Value,
    DateOnly Month,
    SplitType SplitType,
    PersonDto PaidBy,
    List<TransactionShareDto> SharedWith
);

public record TransactionShareDto(PersonDto Person, decimal Amount);
