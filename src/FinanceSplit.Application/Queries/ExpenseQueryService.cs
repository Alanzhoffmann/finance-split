using FinanceSplit.Application.Mapping;
using FinanceSplit.Contracts.Responses;
using FinanceSplit.Domain.Repositories;
using FinanceSplit.Domain.Services;

namespace FinanceSplit.Application.Queries;

public class ExpenseQueryService(ITransactionRepository transactionRepository)
{
    public async Task<MonthlyExpenseSummaryResponse> GetMonthlySummaryAsync(DateOnly month, CancellationToken ct = default)
    {
        var oneOff = await transactionRepository.GetByMonthAsync(month, ct);
        var recurring = await transactionRepository.GetRecurringForMonthAsync(month, ct);

        var allTransactions = oneOff.Concat(recurring).ToList();
        if (allTransactions.Count == 0)
        {
            return new MonthlyExpenseSummaryResponse(Guid.Empty, month, [], new Dictionary<PersonResponse, decimal>(), []);
        }

        var summary = ExpenseCalculator.BuildMonthlySummary(month, allTransactions);
        return summary.ToResponse();
    }

    public async Task<IReadOnlyCollection<ExpenseSettlementResponse>> GetSettlementsAsync(DateOnly month, CancellationToken ct = default)
    {
        var oneOff = await transactionRepository.GetByMonthAsync(month, ct);
        var recurring = await transactionRepository.GetRecurringForMonthAsync(month, ct);

        var allTransactions = oneOff.Concat(recurring).ToList();
        if (allTransactions.Count == 0)
        {
            return [];
        }

        var settlements = ExpenseCalculator.CalculateSettlements(month, allTransactions);
        return settlements.Select(s => s.ToResponse()).ToList();
    }
}
