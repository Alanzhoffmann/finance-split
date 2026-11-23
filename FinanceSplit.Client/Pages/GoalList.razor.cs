using System.Globalization;
using FinanceSplit.Client.ViewModels;

namespace FinanceSplit.Client.Pages;

public partial class GoalList(GoalListViewModel viewModel)
{
    private static readonly CultureInfo _enGb = CultureInfo.GetCultureInfo("en-GB");

    public GoalListViewModel ViewModel { get; } = viewModel;

    protected override async Task OnInitializedAsync()
    {
        await ViewModel.InitializeAsync();
    }
}
