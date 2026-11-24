using FinanceSplit.Contracts.Goals.DTOs;
using FinanceSplit.Contracts.Goals.Services;
using Microsoft.AspNetCore.Components;

namespace FinanceSplit.Client.Components;

public partial class GoalSelect(IGoalService goalService)
{
    [PersistentState(AllowUpdates = true)]
    public IReadOnlyList<GoalDto>? Goals { get; set; }

    [Parameter]
    public string Label { get; set; } = "Goal";

    [Parameter]
    public GoalDto? Value { get; set; }

    [Parameter]
    public EventCallback<GoalDto> ValueChanged { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Goals ??= await goalService.GetGoalsAsync() ?? [];
    }
}
