using System.Globalization;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Components;

namespace FinanceSplit.Client.Pages;

public partial class Home(IDashboardService dashboardService, TimeProvider timeProvider)
{
    private static readonly CultureInfo _enGb = CultureInfo.GetCultureInfo("en-GB");

    private DateTime? Month { get; set; } = timeProvider.GetLocalNow().Date;

    [PersistentState(AllowUpdates = true)]
    public DashboardResponse? Data { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        DateOnly? month = Month.HasValue ? DateOnly.FromDateTime(Month.Value) : null;
        Data ??= await dashboardService.GetDashboardAsync(month);
    }
}
