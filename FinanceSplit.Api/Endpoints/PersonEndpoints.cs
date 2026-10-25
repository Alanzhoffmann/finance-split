using FinanceSplit.Contracts.Commands;
using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FinanceSplit.Api.Endpoints;

internal static class PersonEndpoints
{
    public static RouteGroupBuilder MapPersonEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("api/person").WithTags("Person");

        group.MapGet(
            "",
            async (IPersonService service, CancellationToken cancellationToken = default) =>
            {
                var people = await service.GetPeopleAsync(cancellationToken);
                return TypedResults.Ok(people);
            }
        );

        group.MapPost(
            "",
            async (
                CreatePersonCommand command,
                IPersonService service,
                CancellationToken cancellationToken = default
            ) =>
            {
                var person = await service.CreatePersonAsync(command, cancellationToken);
                return TypedResults.Ok(person);
            }
        );

        group.MapPut(
            "{id:guid}",
            async (
                Guid Id,
                UpdatePersonCommand command,
                IPersonService service,
                CancellationToken cancellationToken
            ) =>
            {
                var person = await service.UpdatePersonAsync(
                    command with
                    {
                        Id = Id,
                    },
                    cancellationToken
                );

                return person is null ? Results.NotFound() : TypedResults.Ok(person);
            }
        );

        return group;
    }
}
