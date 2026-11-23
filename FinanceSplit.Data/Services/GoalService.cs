using System.Xml.Linq;
using FinanceSplit.Contracts.Goals.Commands;
using FinanceSplit.Contracts.Goals.DTOs;
using FinanceSplit.Contracts.Goals.Services;
using FinanceSplit.Contracts.Transactions.Enums;
using FinanceSplit.Domain.Entities;
using FinanceSplit.Domain.Mappers;
using Microsoft.EntityFrameworkCore;

namespace FinanceSplit.Data.Services;

internal sealed class GoalService(AppDbContext context) : IGoalService
{
    public async Task<IReadOnlyList<GoalDto>?> GetGoalsAsync(
        CancellationToken cancellationToken = default
    )
    {
        var goals = await context
            .Goals.AsNoTrackingWithIdentityResolution()
            .ToListAsync(cancellationToken: cancellationToken);

        return [.. goals.Select(g => g.ToDto())];
    }

    public async Task<GoalDto?> CreateGoalAsync(
        CreateGoalCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var people = await context
            .People.Where(p => command.SharedWithIds.Contains(p.Id))
            .ToListAsync(cancellationToken: cancellationToken);

        var newGoal = new Goal
        {
            Name = command.Name,
            Value = command.Amount,
            InitialMonth = command.InitialMonth,
            FinalMonth = command.FinalMonth,
            SplitType = command.SplitType,
            SharedWith = people,
        };

        context.Add(newGoal);
        await context.SaveChangesAsync(cancellationToken);

        return newGoal.ToDto();
    }

    public async Task<GoalDto?> UpdateGoalAsync(
        UpdateGoalCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var existingGoal = await context.Goals.FindAsync([command.Id], cancellationToken);

        if (existingGoal is null)
        {
            return null;
        }

        if (!existingGoal.SharedWith.Select(p => p.Id).SequenceEqual(command.SharedWithIds))
        {
            var people = await context
                .People.Where(p => command.SharedWithIds.Contains(p.Id))
                .ToListAsync(cancellationToken: cancellationToken);

            if (people.Count < command.SharedWithIds.Count)
            {
                // Not every person shared with is accounted for
                return null;
            }

            existingGoal.UpdateSharedWith(people);
        }

        existingGoal.Update(
            command.Name,
            command.Amount,
            command.InitialMonth,
            command.FinalMonth,
            command.SplitType
        );

        await context.SaveChangesAsync(cancellationToken);

        return existingGoal.ToDto();
    }

    public async Task DeleteGoalAsync(
        DeleteGoalCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var goalToDelete = await context.Goals.FindAsync([command.Id], cancellationToken);

        if (goalToDelete is null)
        {
            // TODO figure out 404s here
            return;
        }

        context.Remove(goalToDelete);

        await context.SaveChangesAsync(cancellationToken);
    }
}
