using FinanceSplit.Application.Mapping;
using FinanceSplit.Contracts.Enums;
using FinanceSplit.Contracts.Responses;
using FinanceSplit.Domain.Entities;
using FinanceSplit.Domain.Repositories;
using FinanceSplit.Domain.ValueObjects;

namespace FinanceSplit.Application.Services;

public class TransactionService(ITransactionRepository transactionRepository, IPersonRepository personRepository)
{
    public async Task<TransactionResponse?> CreateTransactionAsync(
        string title,
        decimal amount,
        Guid paidById,
        SplitType splitType,
        IReadOnlyList<Guid> participantIds,
        DateTime? date = null,
        Recurrence? recurrence = null,
        CancellationToken ct = default
    )
    {
        var paidBy = await personRepository.GetByIdAsync(paidById, ct);
        if (paidBy is null)
        {
            return null;
        }

        var participants = new List<Person>();
        foreach (var pid in participantIds)
        {
            var p = await personRepository.GetByIdAsync(pid, ct);
            if (p is null)
            {
                return null;
            }

            participants.Add(p);
        }

        var splitPay = splitType switch
        {
            SplitType.None => SplitPay.CreateNoneSplit(paidBy),
            SplitType.Even => SplitPay.CreateEvenSplit(participants),
            SplitType.Ratio => SplitPay.CreateRatioSplit(participants),
            _ => SplitPay.CreateEvenSplit(participants),
        };

        var transaction = new Transaction(title, amount, paidBy, splitPay, date, recurrence);
        await transactionRepository.AddAsync(transaction, ct);
        await transactionRepository.SaveChangesAsync(ct);
        return transaction.ToResponse();
    }

    public async Task<TransactionResponse?> GetTransactionAsync(Guid id, CancellationToken ct = default)
    {
        var transaction = await transactionRepository.GetByIdAsync(id, ct);
        return transaction?.ToResponse();
    }

    public async Task<TransactionResponse?> UpdateTransactionAsync(
        Guid id,
        string title,
        decimal amount,
        Guid paidById,
        SplitType splitType,
        IReadOnlyList<Guid> participantIds,
        DateTime? date = null,
        CancellationToken ct = default
    )
    {
        var transaction = await transactionRepository.GetByIdAsync(id, ct);
        if (transaction is null)
        {
            return null;
        }

        var paidBy = await personRepository.GetByIdAsync(paidById, ct);
        if (paidBy is null)
        {
            return null;
        }

        var participants = new List<Person>();
        foreach (var pid in participantIds)
        {
            var p = await personRepository.GetByIdAsync(pid, ct);
            if (p is null)
            {
                return null;
            }

            participants.Add(p);
        }

        var splitPay = splitType switch
        {
            SplitType.None => SplitPay.CreateNoneSplit(paidBy),
            SplitType.Even => SplitPay.CreateEvenSplit(participants),
            SplitType.Ratio => SplitPay.CreateRatioSplit(participants),
            _ => SplitPay.CreateEvenSplit(participants),
        };

        transaction.Update(title, amount, paidBy, splitPay, date);
        await transactionRepository.SaveChangesAsync(ct);
        return transaction.ToResponse();
    }
}
