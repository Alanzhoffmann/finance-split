using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FinanceSplit.Client.Pages;

[INotifyPropertyChanged]
public partial class Home(
    IDashboardService dashboardService,
    IBackupService backupService,
    TimeProvider timeProvider
)
{
    private static readonly CultureInfo _enGb = CultureInfo.GetCultureInfo("en-GB");

    [ObservableProperty]
    private partial DateTime? Month { get; set; } = timeProvider.GetLocalNow().Date;

    [PersistentState(AllowUpdates = true)]
    public DashboardResponse? Data { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        DateOnly? month = Month.HasValue ? DateOnly.FromDateTime(Month.Value) : null;
        Data ??= await dashboardService.GetDashboardAsync(month);
    }

    async partial void OnMonthChanged(DateTime? value)
    {
        DateOnly? month = value.HasValue ? DateOnly.FromDateTime(value.Value) : null;
        Data = await dashboardService.GetDashboardAsync(month);
    }

    private async Task UploadFiles(IBrowserFile? file)
    {
        if (file is null)
        {
            return;
        }

        using var stream = file.OpenReadStream();
        await backupService.ImportBackupAsync(stream, file.ContentType, file.Name);
    }
}
