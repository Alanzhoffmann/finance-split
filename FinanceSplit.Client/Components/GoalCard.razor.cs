using System.Globalization;
using FinanceSplit.Client.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FinanceSplit.Client.Components;

public partial class GoalCard
{
    private static readonly CultureInfo _enGb = CultureInfo.GetCultureInfo("en-GB");

    [Parameter]
    [EditorRequired]
    public GoalViewModel ViewModel { get; set; }
}
