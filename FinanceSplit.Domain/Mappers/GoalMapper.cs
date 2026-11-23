using FinanceSplit.Contracts.Goals.DTOs;
using FinanceSplit.Domain.Entities;
using FinanceSplit.Domain.Extensions;

namespace FinanceSplit.Domain.Mappers;

public static class GoalMapper
{
    public static GoalDto ToDto(this Goal goal)
    {
        var months = goal.Months.ToList();
        var calculatedShares = goal.CalculateShares(months);
        var completedShares = goal
            .Transactions.SelectMany(t => t.CalculateShares(t.Month))
            .GroupBy(k => k.Key)
            .ToDictionary(k => k.Key, k => k.Sum(k1 => k1.Value));

        // TODO simplify shape to have per person amount/completed/total

        return new GoalDto(
            goal.Id,
            goal.Name,
            goal.FinalMonth,
            goal.Value,
            goal.SharedWith.ToSharedWithDto(calculatedShares),
            goal.SharedWith.ToSharedWithDto(completedShares)
        );
    }
}
