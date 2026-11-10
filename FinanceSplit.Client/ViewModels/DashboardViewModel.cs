using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FinanceSplit.Client.ViewModels;

public partial class DashboardViewModel(
    IDashboardService dashboardService,
    IBackupService backupService,
    NavigationManager navigationManager,
    TimeProvider timeProvider
) : ObservableObject
{
    public const string MonthParamName = "month";

    [ObservableProperty]
    public partial DateOnly CurrentMonth { get; set; }

    [PersistentState(AllowUpdates = true)]
    public IReadOnlyList<DashboardPersonResponse>? People { get; set; }

    [PersistentState(AllowUpdates = true)]
    public IReadOnlyList<ExpenseSettlementResponse>? Settlements { get; set; }

    public bool IsLoaded => People is not null && Settlements is not null;

    public async Task InitializeAsync(DateOnly? targetMonth = null)
    {
        if (targetMonth is null)
        {
            var today = timeProvider.GetLocalNow().Date;
            targetMonth = new DateOnly(today.Year, today.Month, 1);
        }

        if (CurrentMonth == targetMonth && IsLoaded)
        {
            return;
        }

        CurrentMonth = targetMonth.Value;
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

        var newUri = navigationManager.GetUriWithQueryParameter(
            MonthParamName,
            $"{newMonth:yyyy-MM}"
        );
        navigationManager.NavigateTo(newUri);
    }

    [RelayCommand]
    private async Task UploadBackupAsync(IBrowserFile? file)
    {
        if (file is null)
        {
            return;
        }

        using var stream = file.OpenReadStream();
        await backupService.ImportBackupAsync(stream, file.ContentType, file.Name);
    }
}
