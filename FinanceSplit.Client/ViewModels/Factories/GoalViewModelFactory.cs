using CommunityToolkit.Mvvm.Messaging;
using FinanceSplit.Client.Interfaces;
using FinanceSplit.Contracts.Goals.DTOs;
using FinanceSplit.Contracts.Goals.Services;

namespace FinanceSplit.Client.ViewModels.Factories;

public sealed class GoalViewModelFactory(
    IGoalService goalService,
    IGoalDialogService dialogService,
    IMessenger messenger
)
{
    public GoalViewModel CreateGoalViewModel(GoalDto? dto = null)
    {
        var viewModel = new GoalViewModel(goalService, dialogService, messenger);
        viewModel.Initialize(dto);

        return viewModel;
    }
}
