using System.Globalization;
using FinanceSplit.Client.ViewModels;
using FinanceSplit.Contracts.Dashboard.DTOs;
using Microsoft.AspNetCore.Components;

namespace FinanceSplit.Client.Pages;

public partial class GoalList(GoalListViewModel viewModel)
{
    private static readonly CultureInfo _enGb = CultureInfo.GetCultureInfo("en-GB");

    [Parameter]
    [EditorRequired]
    public IReadOnlyList<GoalDto>? Goals { get; set; }

    public GoalListViewModel ViewModel { get; } = viewModel;

    protected override void OnParametersSet()
    {
        ViewModel.Initialize(Goals);
    }
}
