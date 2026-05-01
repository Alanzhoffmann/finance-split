using FinanceSplit.Application.Services;
using FinanceSplit.Contracts.Requests;

namespace FinanceSplit.Api.Endpoints;

public static class ImportEndpoints
{
    public static void MapImportEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/import").WithTags("Import");

        group.MapPost(
            "/backup",
            async (ImportBackupRequest request, ImportService service, CancellationToken ct) =>
            {
                var result = await service.ImportAsync(request, ct);
                return Results.Ok(result);
            }
        );
    }
}
