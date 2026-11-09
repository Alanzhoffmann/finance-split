using CommunityToolkit.Mvvm.ComponentModel;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Components;

namespace FinanceSplit.Client.ViewModels;

public partial class DashboardViewModel(
    IDashboardService dashboardService,
    NavigationManager navigationManager
) : ObservableObject
{
    [ObservableProperty]
    public partial DateOnly CurrentMonth { get; set; }

    [PersistentState(AllowUpdates = true)]
    public IReadOnlyList<DashboardPersonResponse>? People { get; set; }

    [PersistentState(AllowUpdates = true)]
    public IReadOnlyList<ExpenseSettlementResponse>? Settlements { get; set; }

    public bool IsLoaded => People is not null && Settlements is not null;

    public async Task LoadMonthAsync(DateOnly targetMonth)
    {
        if (CurrentMonth == targetMonth && IsLoaded)
        {
            return;
        }

        CurrentMonth = targetMonth;
        People = null;
        Settlements = null;

        var data = await dashboardService.GetDashboardAsync(CurrentMonth);

        if (data is null)
        {
            return;
        }

        People = data.People;
        Settlements = data.Settlements;
    }

    public void ChangeMonth(DateOnly newMonth)
    {
        if (newMonth == CurrentMonth)
        {
            return;
        }

        navigationManager.NavigateTo($"/{newMonth:yyyy-MM}");
    }
}
