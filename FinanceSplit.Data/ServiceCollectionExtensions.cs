using FinanceSplit.Data.BackgroundServices;
using FinanceSplit.Data.Interfaces;
using FinanceSplit.Data.Internal;
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
            }
        );

        services.AddSingleton<MigrationState>();
        services.AddSingleton<IMigrationState>(sp => sp.GetRequiredService<MigrationState>());

        services.AddHostedService<MigrationBackgroundService>();

        return services;
    }
}
