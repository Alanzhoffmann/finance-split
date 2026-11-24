using System.Diagnostics.CodeAnalysis;
using FinanceSplit.Contracts.Goals.DTOs;
using FinanceSplit.Contracts.Goals.Services;
using Microsoft.AspNetCore.Components;

namespace FinanceSplit.Client.ViewModels;

public class GoalListViewModel(IGoalService goalService)
{
    [PersistentState(AllowUpdates = true)]
    public IReadOnlyList<GoalModel>? Goals { get; set; }

    [MemberNotNullWhen(true, nameof(Goals))]
    public bool IsLoaded => Goals is not null;

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (IsLoaded)
        {
            return;
        }

        var data = await goalService.GetGoalsAsync(cancellationToken);
        if (data is null)
        {
            Goals = [];
            return;
        }

        Goals = [.. data.Select(g => new GoalModel(g))];
    }

    public record GoalModel(
        string Title,
        DateOnly Month,
        decimal Amount,
        IReadOnlyList<GoalPersonModel> People
    )
    {
        public GoalModel(GoalDto dto)
            : this(
                dto.Name,
                dto.Month,
                dto.Amount,
                [
                    .. dto.SharedWith.Select(item => new GoalPersonModel(
                        item.Person.Name,
                        new(
                            item.Amount,
                            dto.Completed.FirstOrDefault(c => c.Person.Id == item.Person.Id)?.Amount
                                ?? 0
                        )
                    )),
                ]
            ) { }

        public GoalProgressModel Progress => new(Amount, People.Sum(p => p.Progress.Completed));
    }

    public record GoalPersonModel(string Name, GoalProgressModel Progress);

    public record GoalProgressModel(decimal Amount, decimal Completed)
    {
        public int Percentage => (int)Math.Floor(Completed / Amount * 100);
    }
}
