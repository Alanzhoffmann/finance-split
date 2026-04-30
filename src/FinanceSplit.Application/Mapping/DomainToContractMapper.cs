using FinanceSplit.Contracts.Enums;
using FinanceSplit.Contracts.Responses;
using FinanceSplit.Domain.Entities;
using FinanceSplit.Domain.ValueObjects;

namespace FinanceSplit.Application.Mapping;

public static class DomainToContractMapper
{
    public static PersonResponse ToResponse(this Person person) => new(person.Id, person.Name, person.Salaries.Select(s => s.ToResponse()).ToList());

    public static SalaryResponse ToResponse(this Salary salary) => new(salary.Date, salary.Amount);

    public static TransactionResponse ToResponse(this Transaction transaction) =>
        new(
            transaction.Id,
            transaction.Title,
            transaction.Description,
            transaction.Amount,
            transaction.Date,
            transaction.PaidBy.ToResponse(),
            transaction.SplitPay.SplitType,
            transaction.SplitPay.People.Select(p => p.ToResponse()).ToList(),
            transaction.Recurrence?.ToResponse()
        );

    public static RecurrenceResponse ToResponse(this Recurrence recurrence) =>
        new(
            recurrence.StartMonth,
            recurrence.Termination switch
            {
                RecurrenceTermination.Open => RecurrenceTerminationType.Open,
                RecurrenceTermination.ByCount => RecurrenceTerminationType.ByCount,
                RecurrenceTermination.ByDate => RecurrenceTerminationType.ByDate,
                _ => RecurrenceTerminationType.Open,
            },
            recurrence.EndMonth,
            recurrence.OccurrenceCount
        );

    public static ExpenseSettlementResponse ToResponse(this ExpenseSettlement settlement) =>
        new(settlement.From.ToResponse(), settlement.To.ToResponse(), settlement.Amount);

    public static MonthlyExpenseSummaryResponse ToResponse(this MonthlyExpenseSummary summary)
    {
        var balances = summary.CalculateNetBalances();
        var settlements = summary.CalculateSettlements();

        return new(
            summary.Id,
            summary.Month,
            summary.Transactions.Select(t => t.ToResponse()).ToList(),
            balances.ToDictionary(kv => kv.Key.ToResponse(), kv => kv.Value),
            settlements.Select(s => s.ToResponse()).ToList()
        );
    }
}
