using System.Globalization;
using FinanceSplit.Client.ViewModels;
using FinanceSplit.Client.ViewModels.Factories;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FinanceSplit.Client.Dialogs;

public partial class GoalDialog(GoalViewModelFactory viewModelFactory)
{
    private static readonly CultureInfo _enGb = CultureInfo.GetCultureInfo("en-GB");

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter]
    public GoalViewModel ViewModel
    {
        get => field ??= viewModelFactory.CreateGoalViewModel();
        set;
    }

    private string Title => ViewModel.IsEditing ? $"Edit goal {ViewModel.Name}" : "Create goal";

    private string SubmitLabel => ViewModel.IsEditing ? "Edit" : "Create";

    private void Cancel() => MudDialog.Cancel();

    private async Task SaveGoal()
    {
        var goalSaved = await ViewModel.SaveGoalAsync();

        if (goalSaved is not null)
        {
            MudDialog.Close(DialogResult.Ok(goalSaved));
        }
    }
}
