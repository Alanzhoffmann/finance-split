using FinanceSplit.Contracts.Goals.DTOs;
using FinanceSplit.Contracts.Goals.Services;

namespace FinanceSplit.Client.ViewModels.Factories;

public sealed class GoalViewModelFactory(IGoalService goalService)
{
    public GoalViewModel CreateGoalViewModel(GoalDto? dto = null)
    {
        var viewModel = new GoalViewModel(goalService);
        viewModel.Initialize(dto);

        return viewModel;
    }
}
