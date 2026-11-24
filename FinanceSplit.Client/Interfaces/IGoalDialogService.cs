using FinanceSplit.Client.ViewModels;
using FinanceSplit.Contracts.Goals.DTOs;

namespace FinanceSplit.Client.Interfaces;

public interface IGoalDialogService
{
    Task<GoalDto?> CreateGoalAsync();
    Task<GoalDto?> EditGoalAsync(GoalViewModel viewModel);
    Task<bool> ShowDeleteGoalConfirmationAsync(GoalDto goal);
}
