using System.Globalization;
using FinanceSplit.Client.ViewModels;
using FinanceSplit.Contracts.People.DTOs;
using FinanceSplit.Contracts.People.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FinanceSplit.Client.Dialogs;

public partial class IncomeRecordDialog(IncomeRecordViewModel viewModel)
{
    private static readonly CultureInfo _enGb = CultureInfo.GetCultureInfo("en-GB");

    private static IReadOnlyList<IncomeType> IncomeTypes => Enum.GetValues<IncomeType>();

    [Parameter]
    [EditorRequired]
    public PersonDto Person { get; set; }

    [Parameter]
    public IncomeRecordDto? IncomeRecord { get; set; }

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    private IncomeRecordViewModel ViewModel { get; } = viewModel;

    private string Title =>
        ViewModel.IsEditing
            ? $"Edit income {ViewModel.Month} {ViewModel.IncomeType}"
            : "Add income";

    private string SubmitLabel => ViewModel.IsEditing ? "Edit" : "Add";

    private void Cancel() => MudDialog.Cancel();

    protected override void OnParametersSet()
    {
        ViewModel.Initialize(Person, IncomeRecord);
    }

    private async Task SaveIncome()
    {
        var savedIncome = await ViewModel.SaveIncomeAsync();

        if (savedIncome is not null)
        {
            MudDialog.Close(DialogResult.Ok(savedIncome));
        }
    }
}
