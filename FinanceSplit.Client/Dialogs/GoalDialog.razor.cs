using System.Globalization;
using FinanceSplit.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FinanceSplit.Client.Dialogs;

public partial class GoalDialog
{
    private static readonly CultureInfo _enGb = CultureInfo.GetCultureInfo("en-GB");

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter]
    public GoalViewModel ViewModel { get; set; } = default!;

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
