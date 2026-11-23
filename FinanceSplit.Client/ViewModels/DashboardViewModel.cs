using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceSplit.Contracts.Dashboard.DTOs;
using FinanceSplit.Contracts.Dashboard.Services;
using FinanceSplit.Contracts.FileImport.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FinanceSplit.Client.ViewModels;

public partial class DashboardViewModel(
    IDashboardService dashboardService,
    IFileImportService backupService,
    NavigationManager navigationManager,
    TimeProvider timeProvider
) : ObservableObject
{
    public const string MonthParamName = "month";

    [ObservableProperty]
    public partial DateOnly CurrentMonth { get; set; }

    [PersistentState(AllowUpdates = true)]
    public IReadOnlyList<PersonSummaryDto>? People { get; set; }

    [PersistentState(AllowUpdates = true)]
    public IReadOnlyList<ExpenseSettlementDto>? Settlements { get; set; }

    [PersistentState(AllowUpdates = true)]
    public IReadOnlyList<GoalDto>? Goals { get; set; }

    [MemberNotNullWhen(true, nameof(People), nameof(Settlements), nameof(Goals))]
    public bool IsLoaded => People is not null && Settlements is not null && Goals is not null;

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
        Goals = null;

        var data = await dashboardService.GetDashboardAsync(CurrentMonth);

        if (data is null)
        {
            return;
        }

        People = data.MonthSummary.People;
        Settlements = data.MonthSummary.Settlements;
        Goals = data.Goals;
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
