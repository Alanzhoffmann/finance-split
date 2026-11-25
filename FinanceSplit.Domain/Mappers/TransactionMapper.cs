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
            transaction.PaidBy.ToListDto(),
            transaction.SharedWith.ToSharedWithDto(calculatedShares),
            transaction.Goal?.ToListDto()
        );
    }

    public static IReadOnlyList<TransactionShareDto> ToSharedWithDto(
        this IEnumerable<Person> sharedWith,
        IReadOnlyDictionary<Person, decimal> calculatedShares
    ) =>
        [
            .. sharedWith.Select(s => new TransactionShareDto(
                s.ToListDto(),
                calculatedShares.GetValueOrDefault(s)
            )),
        ];
}
