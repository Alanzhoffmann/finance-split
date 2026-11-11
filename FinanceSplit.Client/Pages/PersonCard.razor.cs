using System.Globalization;
using FinanceSplit.Client.ViewModels;
using FinanceSplit.Contracts.DTOs;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FinanceSplit.Client.Pages;

public partial class PersonCard(PersonViewModel viewModel)
{
    private static readonly CultureInfo _enGb = CultureInfo.GetCultureInfo("en-GB");
    private readonly TableGroupDefinition<IncomeRecordResponse> _groupDefinition = new()
    {
        GroupName = "Month",
        Indentation = true,
        Expandable = false,
        Selector = (e) => e.Month.ToString("MMM/yyyy"),
    };

    [Parameter]
    [EditorRequired]
    public PersonResponse Person { get; set; }

    private PersonViewModel ViewModel { get; set; } = viewModel;

    protected override void OnParametersSet() => ViewModel.Initialize(Person);
}
