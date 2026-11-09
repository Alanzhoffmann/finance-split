using System.ComponentModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using FinanceSplit.Client.ViewModels;
using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FinanceSplit.Client.Pages;

[INotifyPropertyChanged]
public partial class Home(
    DashboardViewModel viewModel,
    IBackupService backupService,
    TimeProvider timeProvider
) : IDisposable
{
    private static readonly CultureInfo _enGb = CultureInfo.GetCultureInfo("en-GB");
    private bool _disposedValue;

    [Parameter]
    public string? MonthParam { get; set; }

    [ObservableProperty]
    private partial DateTime? Month { get; set; } = timeProvider.GetLocalNow().Date;

    private DashboardViewModel ViewModel { get; set; } = viewModel;

    protected override void OnInitialized() =>
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e) =>
        InvokeAsync(StateHasChanged);

    protected override async Task OnParametersSetAsync()
    {
        if (
            string.IsNullOrWhiteSpace(MonthParam)
            || !DateOnly.TryParseExact(MonthParam, "yyyy-MM", out var targetMonth)
        )
        {
            var today = timeProvider.GetLocalNow().Date;
            targetMonth = new DateOnly(today.Year, today.Month, 1);
        }
        else
        {
            targetMonth = new DateOnly(targetMonth.Year, targetMonth.Month, 1);
        }

        await ViewModel.LoadMonthAsync(targetMonth);
    }

    private void OnMonthSelected(DateTime? selectedDate)
    {
        if (selectedDate.HasValue)
        {
            var newMonth = DateOnly.FromDateTime(selectedDate.Value);
            ViewModel.ChangeMonth(newMonth);
        }
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

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            ViewModel = null!;
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
