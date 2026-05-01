using FinanceSplit.Application.Queries;
using FinanceSplit.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceSplit.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<PersonService>();
        services.AddScoped<TransactionService>();
        services.AddScoped<ImportService>();
        services.AddScoped<ExpenseQueryService>();

        return services;
    }
}
