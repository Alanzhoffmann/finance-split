using FinanceSplit.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FinanceSplit.Data.BackgroundServices;

public class MigrationBackgroundService(IServiceProvider serviceProvider, ILogger<MigrationBackgroundService> logger, IMigrationState migrationState)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogWaitingForMigrations();

            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FinanceSplitDbContext>();
            await dbContext.Database.MigrateAsync(stoppingToken);

            logger.LogMigrationsCompleted();
            migrationState.IsDone = true;
        }
        catch (Exception ex)
        {
            logger.LogMigrationError(ex);
        }
    }
}

internal static partial class MigrationLoggerExtensions
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Waiting for database migrations to complete...")]
    public static partial void LogWaitingForMigrations(this ILogger logger);

    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Database migrations completed successfully.")]
    public static partial void LogMigrationsCompleted(this ILogger logger);

    [LoggerMessage(EventId = 2, Level = LogLevel.Critical, Message = "Failed to apply database migrations.")]
    public static partial void LogMigrationError(this ILogger logger, Exception ex);
}
