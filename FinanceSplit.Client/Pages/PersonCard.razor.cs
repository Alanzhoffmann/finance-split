using FinanceSplit.Contracts.DTOs;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FinanceSplit.Client.Pages;

public partial class PersonCard
{
    [Parameter]
    [EditorRequired]
    public required PersonResponse Person { get; set; }

    private readonly TableGroupDefinition<IncomeRecordResponse> _groupDefinition = new()
    {
        GroupName = "Month",
        Indentation = true,
        Expandable = false,
        Selector = (e) => e.Month.ToString("MMM/yyyy"),
    };
}
