using System.Text.Json;
using System.Text.Json.Serialization;
using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FinanceSplit.Api.Endpoints;

internal static class BackupEndpoints
{
    private static readonly JsonSerializerOptions _exportJsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public static RouteGroupBuilder MapBackupEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("api/backup").WithTags("Backup").DisableAntiforgery();

        group.MapGet(
            "export/download",
            async (IBackupService service, CancellationToken cancellationToken = default) =>
            {
                var backup = await service.GetBackupAsync();
                var fileContents = JsonSerializer.SerializeToUtf8Bytes(backup, _exportJsonOptions);
                var fileName = $"finance-split-backup-{backup.ExportedAtUtc:yyyyMMdd-HHmmss}.json";
                return Results.File(fileContents, "application/json", fileName);
            }
        );

        group.MapPost(
            "import",
            async (
                [FromForm] IFormFile file,
                IBackupService service,
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
