using FinanceSplit.Contracts.Transactions.Enums;
using Microsoft.AspNetCore.Components;

namespace FinanceSplit.Client.Components;

public partial class SplitTypeSelect
{
    private static IReadOnlyList<SplitType> SplitTypes => Enum.GetValues<SplitType>();

    [Parameter]
    public SplitType Value { get; set; } = SplitType.None;

    [Parameter]
    public string? Label { get; set; }

    [Parameter]
    public EventCallback<SplitType> ValueChanged { get; set; }
}
