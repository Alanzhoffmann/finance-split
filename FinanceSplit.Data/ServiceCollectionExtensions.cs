using FinanceSplit.Contracts.Services;
using FinanceSplit.Data.BackgroundServices;
using FinanceSplit.Data.Interceptors;
using FinanceSplit.Data.Interfaces;
using FinanceSplit.Data.Internal;
using FinanceSplit.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceSplit.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(
            (sp, options) =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                options.UseSqlite(connectionString);

                options.AddInterceptors(sp.GetRequiredService<CalculatorInterceptor>());
            }
        );

        services.AddTransient<CalculatorInterceptor>();

        services.AddSingleton<MigrationState>();
        services.AddSingleton<IMigrationState>(sp => sp.GetRequiredService<MigrationState>());

        services.AddHostedService<MigrationBackgroundService>();

        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IBackupService, BackupService>();

        services.AddSingleton<CalculatorBackgroundService>();
        services.AddHostedService(sp => sp.GetRequiredService<CalculatorBackgroundService>());

        return services;
    }
}
