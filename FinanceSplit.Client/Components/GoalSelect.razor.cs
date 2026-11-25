using FinanceSplit.Contracts.Goals.DTOs;
using FinanceSplit.Contracts.Goals.Services;
using Microsoft.AspNetCore.Components;

namespace FinanceSplit.Client.Components;

public partial class GoalSelect(IGoalService goalService)
{
    [PersistentState(AllowUpdates = true)]
    public IReadOnlyList<GoalListDto>? Goals { get; set; }

    [Parameter]
    public string Label { get; set; } = "Goal";

    [Parameter]
    public GoalListDto? Value { get; set; }

    [Parameter]
    public EventCallback<GoalListDto> ValueChanged { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Goals ??= await goalService.GetGoalsListAsync() ?? [];
    }
}
