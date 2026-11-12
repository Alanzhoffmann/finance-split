using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Domain.Entities;

namespace FinanceSplit.Domain.Mappers;

public static class TransactionMapper
{
    public static TransactionDto ToDto(this Transaction transaction)
    {
        var calculatedShares = transaction.CalculateShares();

        return new TransactionDto(
            transaction.Id,
            transaction.Name,
            transaction.Value,
            transaction.Month,
            transaction.SplitType,
            new TransactionPersonDto(transaction.PaidBy.Id, transaction.PaidBy.Name),
            [
                .. transaction.SharedWith.Select(s => new TransactionShareDto(
                    new TransactionPersonDto(s.Id, s.Name),
                    calculatedShares.GetValueOrDefault(s)
                )),
            ]
        );
    }
}
