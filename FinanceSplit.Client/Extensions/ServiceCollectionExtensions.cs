using FinanceSplit.Client.ViewModels;

namespace FinanceSplit.Client.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddViewModels(this IServiceCollection services)
    {
        services.AddTransient<TransactionViewModel>();
        services.AddTransient<DashboardViewModel>();
    }
}
