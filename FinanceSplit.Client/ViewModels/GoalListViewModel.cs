using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.Input;
using FinanceSplit.Client.Interfaces;
using FinanceSplit.Client.ViewModels.Factories;
using FinanceSplit.Contracts.Goals.Services;
using Microsoft.AspNetCore.Components;

namespace FinanceSplit.Client.ViewModels;

public partial class GoalListViewModel(
    IGoalService goalService,
    IGoalDialogService dialogService,
    GoalViewModelFactory viewModelFactory
)
{
    [PersistentState(AllowUpdates = true)]
    public IReadOnlyList<GoalViewModel>? Goals { get; set; }

    [MemberNotNullWhen(true, nameof(Goals))]
    public bool IsLoaded => Goals is not null;

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (IsLoaded)
        {
            return;
        }

        var data = await goalService.GetGoalsAsync(cancellationToken);
        if (data is null)
        {
            Goals = [];
            return;
        }

        Goals = [.. data.Select(viewModelFactory.CreateGoalViewModel)];
    }

    [RelayCommand]
    private async Task CreateGoalAsync()
    {
        var newGoal = await dialogService.CreateGoalAsync();
    }
}
