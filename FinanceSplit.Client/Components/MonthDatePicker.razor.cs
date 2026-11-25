using Microsoft.AspNetCore.Components;

namespace FinanceSplit.Client.Components;

public partial class MonthDatePicker
{
    [Parameter]
    public DateOnly? Month { get; set; }

    [Parameter]
    public string? Label { get; set; }

    [Parameter]
    public EventCallback<DateOnly?> MonthChanged { get; set; }

    DateTime? Date
    {
        get => Month?.ToDateTime(TimeOnly.MinValue);
        set => Month = value is null ? null : DateOnly.FromDateTime(value.Value);
    }

    public async Task OnDateSelectedAsync()
    {
        await MonthChanged.InvokeAsync(Month);
    }
}
