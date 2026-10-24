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

        return group;
    }
}
