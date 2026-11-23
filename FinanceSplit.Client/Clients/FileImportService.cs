using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using FinanceSplit.Contracts.FileImport.Dtos;
using FinanceSplit.Contracts.FileImport.Services;

namespace FinanceSplit.Client.Clients;

internal partial class FileImportService(HttpClient client) : IFileImportService
{
    public Task<FileBackupResponse> GetBackupAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public async Task<FileImportResponse?> ImportBackupAsync(
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

        var response = await client.PostAsync("api/file/import", form, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync(
            BackupSerializerContext.Default.FileImportResponse,
            cancellationToken: cancellationToken
        );
    }

    [JsonSerializable(typeof(FileImportResponse))]
    partial class BackupSerializerContext : JsonSerializerContext;
}
