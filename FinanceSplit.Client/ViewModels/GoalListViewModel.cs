using FinanceSplit.Contracts.Dashboard.DTOs;
using Microsoft.AspNetCore.Components;

namespace FinanceSplit.Client.ViewModels;

public class GoalListViewModel
{
    [PersistentState(AllowUpdates = true)]
    public IReadOnlyList<GoalModel> Goals { get; set; } = [];

    public void Initialize(IEnumerable<GoalDto>? goals)
    {
        if (goals is null)
        {
            return;
        }

        Goals = [.. goals.Select(g => new GoalModel(g))];
    }
}
