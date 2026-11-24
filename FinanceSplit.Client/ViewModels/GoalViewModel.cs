using System.Diagnostics.CodeAnalysis;
using FinanceSplit.Contracts.Goals.Commands;
using FinanceSplit.Contracts.Goals.DTOs;
using FinanceSplit.Contracts.Goals.Services;
using FinanceSplit.Contracts.Transactions.Enums;

namespace FinanceSplit.Client.ViewModels;

public class GoalViewModel(IGoalService goalService)
{
    public void Initialize(GoalDto? dto = null)
    {
        if (dto is not null)
        {
            Id = dto.Id;
            Name = dto.Name;
            Amount = dto.Amount;
            InitialMonth = dto.InitialMonth;
            FinalMonth = dto.FinalMonth;
            SplitType = dto.SplitType;
            SharedWith = [.. dto.SharedWith.Select(s => s.Person.Id)];
            People =
            [
                .. dto.SharedWith.Select(item => new GoalPersonModel(
                    item.Person.Name,
                    new(
                        item.Amount,
                        dto.Completed.FirstOrDefault(c => c.Person.Id == item.Person.Id)?.Amount
                            ?? 0
                    )
                )),
            ];
        }
    }

    internal async Task<GoalDto?> SaveGoalAsync(CancellationToken cancellationToken = default)
    {
        // TODO review validation
        if (InitialMonth is null)
        {
            throw new NullReferenceException("initial month null");
        }

        if (FinalMonth is null)
        {
            throw new NullReferenceException("final month null");
        }

        if (Name is null)
        {
            throw new NullReferenceException("name null");
        }

        if (IsEditing)
        {
            var updateCommand = new UpdateGoalCommand(
                Id.Value,
                Name,
                Amount,
                InitialMonth.Value,
                FinalMonth.Value,
                SplitType,
                SharedWith
            );
            var updatedGoal = await goalService.UpdateGoalAsync(updateCommand, cancellationToken);
            if (updatedGoal is null)
            {
                throw new InvalidOperationException("updated goal null");
            }

            Initialize(updatedGoal);
            return updatedGoal;
        }
        else
        {
            var createCommand = new CreateGoalCommand(
                Name,
                Amount,
                InitialMonth.Value,
                FinalMonth.Value,
                SplitType,
                SharedWith
            );
            var createdGoal = await goalService.CreateGoalAsync(createCommand, cancellationToken);
            if (createdGoal is null)
            {
                throw new InvalidOperationException("created goal null");
            }

            Initialize(createdGoal);
            return createdGoal;
        }
    }

    [MemberNotNullWhen(true, nameof(Id))]
    public bool IsEditing => Id is not null;

    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public decimal Amount { get; set; }
    public DateOnly? InitialMonth { get; set; }
    public DateOnly? FinalMonth { get; set; }
    public SplitType SplitType { get; set; }
    public IReadOnlyList<Guid> SharedWith { get; set; } = [];
    public IReadOnlyList<GoalPersonModel> People { get; set; } = [];

    public GoalProgressModel Progress => new(Amount, People.Sum(p => p.Progress.Completed));
}

public record GoalPersonModel(string Name, GoalProgressModel Progress);

public record GoalProgressModel(decimal Amount, decimal Completed)
{
    public int Percentage => (int)Math.Floor(Completed / Amount * 100);
}
