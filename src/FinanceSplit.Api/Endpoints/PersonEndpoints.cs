using FinanceSplit.Application.Services;
using FinanceSplit.Contracts.Requests;

namespace FinanceSplit.Api.Endpoints;

public static class PersonEndpoints
{
    public static void MapPersonEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/people").WithTags("People");

        group.MapGet(
            "/",
            async (PersonService service, CancellationToken ct) =>
            {
                var people = await service.GetAllPeopleAsync(ct);
                return Results.Ok(people);
            }
        );

        group.MapGet(
            "/{id:guid}",
            async (Guid id, PersonService service, CancellationToken ct) =>
            {
                var person = await service.GetPersonAsync(id, ct);
                return person is null ? Results.NotFound() : Results.Ok(person);
            }
        );

        group.MapPost(
            "/",
            async (CreatePersonRequest request, PersonService service, CancellationToken ct) =>
            {
                var person = await service.CreatePersonAsync(request.Name, ct);
                return Results.Created($"/api/people/{person.Id}", person);
            }
        );

        group.MapPut(
            "/{id:guid}/name",
            async (Guid id, UpdatePersonNameRequest request, PersonService service, CancellationToken ct) =>
            {
                var person = await service.UpdateNameAsync(id, request.Name, ct);
                return person is null ? Results.NotFound() : Results.Ok(person);
            }
        );

        group.MapPost(
            "/{id:guid}/salaries",
            async (Guid id, AddSalaryRequest request, PersonService service, CancellationToken ct) =>
            {
                var person = await service.AddSalaryAsync(id, request.Date, request.Amount, ct);
                return person is null ? Results.NotFound() : Results.Ok(person);
            }
        );
    }
}
