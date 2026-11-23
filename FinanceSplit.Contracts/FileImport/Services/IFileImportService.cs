using FinanceSplit.Contracts.FileImport.DTOs;

namespace FinanceSplit.Contracts.FileImport.Services;

public interface IFileImportService
{
    Task<FileBackupResponse> GetBackupAsync(CancellationToken cancellationToken = default);
    Task<FileImportResponse?> ImportBackupAsync(
        Stream file,
        string contentType,
        string fileName,
        CancellationToken cancellationToken = default
    );
}
