using FinanceSplit.Contracts.Goals.DTOs;
using FinanceSplit.Contracts.People.DTOs;
using FinanceSplit.Contracts.Transactions.Enums;

namespace FinanceSplit.Contracts.Transactions.DTOs;

public record TransactionDto(
    Guid Id,
    string Name,
    decimal Amount,
    DateOnly Month,
    SplitType SplitType,
    PersonDto PaidBy,
    IReadOnlyList<TransactionShareDto> SharedWith,
    GoalListDto? Goal
);

public record TransactionShareDto(PersonDto Person, decimal Amount);
