using FinanceSplit.Contracts.Transactions.DTOs;
using FinanceSplit.Domain.Entities;
using FinanceSplit.Domain.Extensions;

namespace FinanceSplit.Domain.Mappers;

public static class TransactionMapper
{
    public static TransactionDto ToDto(this Transaction transaction)
    {
        var calculatedShares = transaction.CalculateShares(transaction.Month);

        return new TransactionDto(
            transaction.Id,
            transaction.Name,
            transaction.Value,
            transaction.Month,
            transaction.SplitType,
            transaction.PaidBy.ToDto(),
            transaction.SharedWith.ToSharedWithDto(calculatedShares),
            transaction.Goal?.ToSimpleDto()
        );
    }

    public static IReadOnlyList<TransactionShareDto> ToSharedWithDto(
        this IEnumerable<Person> sharedWith,
        IReadOnlyDictionary<Person, decimal> calculatedShares
    ) =>
        [
            .. sharedWith.Select(s => new TransactionShareDto(
                s.ToDto(),
                calculatedShares.GetValueOrDefault(s)
            )),
        ];
}
