using FinanceSplit.Data.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FinanceSplit.Data.BackgroundServices;

internal partial class MigrationBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<MigrationBackgroundService> logger,
    MigrationState migrationState
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            LogWaitingForMigrations();

            await using var scope = scopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var dbPath = dbContext
                .Database.GetConnectionString()
                ?.Replace("Data Source=", "", StringComparison.OrdinalIgnoreCase);
            var dbDir = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(dbDir))
            {
                Directory.CreateDirectory(dbDir);
            }

            await dbContext.Database.MigrateAsync(stoppingToken);

            LogMigrationsCompleted();
            migrationState.IsDone = true;
        }
        catch (Exception ex)
        {
            LogMigrationError(ex);
        }
    }

    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Information,
        Message = "Waiting for database migrations to complete..."
    )]
    public partial void LogWaitingForMigrations();

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Database migrations completed successfully."
    )]
    public partial void LogMigrationsCompleted();

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Critical,
        Message = "Failed to apply database migrations."
    )]
    public partial void LogMigrationError(Exception ex);
}
