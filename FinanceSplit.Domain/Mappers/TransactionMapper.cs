using FinanceSplit.Contracts.People.DTOs;
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
            new PersonDto(transaction.PaidBy.Id, transaction.PaidBy.Name),
            [
                .. transaction.SharedWith.Select(s => new TransactionShareDto(
                    new PersonDto(s.Id, s.Name),
                    calculatedShares.GetValueOrDefault(s)
                )),
            ]
        );
    }
}
