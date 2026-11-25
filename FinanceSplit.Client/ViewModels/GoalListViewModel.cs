using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FinanceSplit.Client.Interfaces;
using FinanceSplit.Client.ViewModels.Factories;
using FinanceSplit.Client.ViewModels.Messages;
using FinanceSplit.Contracts.Goals.Services;
using Microsoft.AspNetCore.Components;

namespace FinanceSplit.Client.ViewModels;

public partial class GoalListViewModel(
    IGoalService goalService,
    IGoalDialogService dialogService,
    GoalViewModelFactory viewModelFactory,
    IMessenger messenger
) : ObservableRecipient(messenger), IRecipient<GoalDeletedMessage>
{
    [PersistentState(AllowUpdates = true)]
    public List<GoalViewModel>? Goals { get; private set; }

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

    public void Receive(GoalDeletedMessage message)
    {
        if (IsLoaded)
        {
            var index = Goals.FindIndex(g => g.Id == message.Id);
            if (index != -1)
            {
                Goals.RemoveAt(index);
            }
        }
    }

    [RelayCommand]
    private async Task CreateGoalAsync()
    {
        var newGoal = await dialogService.CreateGoalAsync();
        if (newGoal is not null)
        {
            Goals = [.. Goals ?? [], viewModelFactory.CreateGoalViewModel(newGoal)];
        }
    }
}
