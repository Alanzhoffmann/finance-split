using FinanceSplit.Contracts.Goals.Commands;
using FinanceSplit.Contracts.Goals.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FinanceSplit.Api.Endpoints;

internal static class GoalEndpoints
{
    public static RouteGroupBuilder MapGoalEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("api/goals").WithTags("Goals");

        group.MapGet(
            "",
            async (IGoalService service, CancellationToken cancellationToken = default) =>
            {
                var goals = await service.GetGoalsAsync(cancellationToken);
                return TypedResults.Ok(goals);
            }
        );

        group.MapGet(
            "list",
            async (IGoalService service, CancellationToken cancellationToken = default) =>
            {
                var goals = await service.GetGoalsListAsync(cancellationToken);
                return TypedResults.Ok(goals);
            }
        );

        group.MapPost(
            "",
            async (
                CreateGoalCommand command,
                IGoalService service,
                CancellationToken cancellationToken = default
            ) =>
            {
                var newGoal = await service.CreateGoalAsync(command, cancellationToken);
                return TypedResults.Ok(newGoal);
            }
        );

        group.MapPut(
            "{id:guid}",
            async (
                Guid id,
                UpdateGoalCommand command,
                IGoalService service,
                CancellationToken cancellationToken = default
            ) =>
            {
                var updatedGoal = await service.UpdateGoalAsync(
                    command with
                    {
                        Id = id,
                    },
                    cancellationToken
                );
                return updatedGoal is null ? Results.NotFound() : TypedResults.Ok(updatedGoal);
            }
        );

        group.MapDelete(
            "{id:guid}",
            async (Guid id, IGoalService service, CancellationToken cancellationToken = default) =>
            {
                await service.DeleteGoalAsync(new(id), cancellationToken);
                return Results.NoContent();
            }
        );

        return group;
    }
}
