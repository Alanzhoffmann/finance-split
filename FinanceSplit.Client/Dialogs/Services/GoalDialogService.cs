using FinanceSplit.Client.Extensions;
using FinanceSplit.Client.Interfaces;
using FinanceSplit.Client.ViewModels;
using FinanceSplit.Contracts.Goals.DTOs;
using MudBlazor;

namespace FinanceSplit.Client.Dialogs.Services;

public sealed class GoalDialogService(IDialogService dialogService) : IGoalDialogService
{
    private readonly DialogOptions _dialogOptions = new() { FullWidth = true };

    public Task<GoalDto?> CreateGoalAsync() => OpenDialogAsync();

    public Task<GoalDto?> EditGoalAsync(GoalViewModel viewModel) => OpenDialogAsync(viewModel);

    public async Task<bool> ShowDeleteGoalConfirmationAsync(GoalDto goal)
    {
        bool? result = await dialogService.ShowMessageBoxAsync(
            "Warning",
            "Deleting can not be undone!",
            yesText: "Delete!",
            cancelText: "Cancel"
        );

        return result == true;
    }

    private async Task<GoalDto?> OpenDialogAsync(GoalViewModel? viewModel = null) =>
        await dialogService.ShowDialogWithResultAsync<GoalDto, GoalDialog>(
            new DialogParameters { ["ViewModel"] = viewModel },
            _dialogOptions
        );
}
