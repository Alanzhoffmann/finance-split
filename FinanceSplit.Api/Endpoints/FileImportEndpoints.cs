using System.Text.Json;
using System.Text.Json.Serialization;
using FinanceSplit.Contracts.FileImport.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FinanceSplit.Api.Endpoints;

internal static class FileImportEndpoints
{
    private static readonly JsonSerializerOptions _exportJsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public static RouteGroupBuilder MapFileImportEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("api/file").WithTags("File").DisableAntiforgery();

        group.MapGet(
            "export/download",
            async (IFileImportService service, CancellationToken cancellationToken = default) =>
            {
                var backup = await service.GetBackupAsync(cancellationToken);
                var fileContents = JsonSerializer.SerializeToUtf8Bytes(backup, _exportJsonOptions);
                var fileName = $"finance-split-backup-{backup.ExportedAtUtc:yyyyMMdd-HHmmss}.json";
                return Results.File(fileContents, "application/json", fileName);
            }
        );

        group.MapPost(
            "import",
            async (
                [FromForm] IFormFile file,
                IFileImportService service,
                CancellationToken cancellationToken = default
            ) =>
            {
                using var stream = file.OpenReadStream();
                var importResult = await service.ImportBackupAsync(
                    stream,
                    file.ContentType,
                    file.Name,
                    cancellationToken
                );
                return importResult is null ? Results.NotFound() : TypedResults.Ok(importResult);
            }
        );

        return group;
    }
}
