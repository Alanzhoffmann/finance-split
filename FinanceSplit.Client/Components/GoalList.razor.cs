using System.Globalization;
using FinanceSplit.Client.ViewModels;

namespace FinanceSplit.Client.Components;

public partial class GoalList(GoalListViewModel viewModel)
{
    public GoalListViewModel ViewModel { get; } = viewModel;

    protected override async Task OnInitializedAsync()
    {
        await ViewModel.InitializeAsync();
    }
}
