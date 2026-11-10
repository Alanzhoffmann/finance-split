using System.Globalization;
using FinanceSplit.Client.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FinanceSplit.Client.Pages;

public partial class TransactionList(TransactionListViewModel viewModel)
{
    private static readonly CultureInfo _enGb = CultureInfo.GetCultureInfo("en-GB");

    [Parameter]
    [SupplyParameterFromQuery(Name = TransactionListViewModel.MonthParamName)]
    public string? MonthParam { get; set; }

    private TransactionListViewModel ViewModel { get; set; } = viewModel;

    protected override async Task OnParametersSetAsync()
    {
        DateOnly? targetMonth = null;
        if (
            !string.IsNullOrWhiteSpace(MonthParam)
            && DateOnly.TryParseExact(MonthParam, "yyyy-MM", out var selectedMonth)
        )
        {
            targetMonth = new DateOnly(selectedMonth.Year, selectedMonth.Month, 1);
        }

        await ViewModel.InitializeAsync(targetMonth);
    }

    private void OnMonthSelected(DateTime? selectedDate)
    {
        if (selectedDate.HasValue)
        {
            var newMonth = DateOnly.FromDateTime(selectedDate.Value);
            ViewModel.ChangeMonth(newMonth);
        }
    }
}
