using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;

namespace FinanceSplit.Client.Clients;

internal partial class BackupService(HttpClient client) : IBackupService
{
    public Task<BackupResponse> GetBackupAsync() => throw new NotImplementedException();

    public async Task<BackupImportResponse?> ImportBackupAsync(
        Stream file,
        string contentType,
        string fileName,
        CancellationToken cancellationToken = default
    )
    {
        using var form = new MultipartFormDataContent();
        using var fileStream = new StreamContent(file);

        fileStream.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        form.Add(fileStream, "file", fileName);

        var response = await client.PostAsync("api/backup/import", form, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync(
            BackupSerializerContext.Default.BackupImportResponse,
            cancellationToken: cancellationToken
        );
    }

    [JsonSerializable(typeof(BackupImportResponse))]
    partial class BackupSerializerContext : JsonSerializerContext;
}
