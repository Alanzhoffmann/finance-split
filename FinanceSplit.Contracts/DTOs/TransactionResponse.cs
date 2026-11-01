using FinanceSplit.Contracts.Enums;

namespace FinanceSplit.Contracts.DTOs;

public record TransactionResponse(
    Guid Id,
    string Name,
    decimal Value,
    DateOnly Date,
    SplitType SplitType,
    TransactionPersonResponse PaidBy,
    List<TransactionPersonResponse> SharedWith
);

public record TransactionPersonResponse(Guid Id, string Name);
