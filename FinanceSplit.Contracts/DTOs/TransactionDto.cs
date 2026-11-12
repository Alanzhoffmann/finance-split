using FinanceSplit.Contracts.Enums;

namespace FinanceSplit.Contracts.DTOs;

public record TransactionDto(
    Guid Id,
    string Name,
    decimal Value,
    DateOnly Date,
    SplitType SplitType,
    TransactionPersonDto PaidBy,
    List<TransactionShareDto> SharedWith
);

public record TransactionPersonDto(Guid Id, string Name);

public record TransactionShareDto(TransactionPersonDto Person, decimal Amount);
