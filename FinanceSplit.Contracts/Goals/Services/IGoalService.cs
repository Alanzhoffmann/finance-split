using FinanceSplit.Contracts.Goals.Commands;
using FinanceSplit.Contracts.Goals.DTOs;

namespace FinanceSplit.Contracts.Goals.Services;

public interface IGoalService
{
    Task<IReadOnlyList<GoalDto>?> GetGoalsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<GoalListDto>?> GetGoalsListAsync(
        CancellationToken cancellationToken = default
    );

    Task<GoalDto?> CreateGoalAsync(
        CreateGoalCommand command,
        CancellationToken cancellationToken = default
    );

    Task<GoalDto?> UpdateGoalAsync(
        UpdateGoalCommand command,
        CancellationToken cancellationToken = default
    );

    Task DeleteGoalAsync(DeleteGoalCommand command, CancellationToken cancellationToken = default);
}
