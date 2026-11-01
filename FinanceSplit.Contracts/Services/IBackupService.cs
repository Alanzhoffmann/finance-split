using FinanceSplit.Contracts.DTOs;

namespace FinanceSplit.Contracts.Services;

public interface IBackupService
{
    Task<BackupResponse> GetBackupAsync();
    Task<BackupImportResponse?> ImportBackupAsync(
        Stream file,
        string contentType,
        string fileName,
        CancellationToken cancellationToken = default
    );
}
