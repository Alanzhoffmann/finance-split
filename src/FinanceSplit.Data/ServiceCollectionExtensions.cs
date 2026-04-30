using FinanceSplit.Data.BackgroundServices;
using FinanceSplit.Data.Interfaces;
using FinanceSplit.Data.Internal;
using FinanceSplit.Data.Repositories;
using FinanceSplit.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceSplit.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<FinanceSplitDbContext>(options => options.UseSqlite(connectionString));

        services.AddSingleton<IMigrationState, MigrationState>();
        services.AddHostedService<MigrationBackgroundService>();

        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        return services;
    }
}
