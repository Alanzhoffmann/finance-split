using CommunityToolkit.Mvvm.Messaging;
using FinanceSplit.Client.Dialogs.Services;
using FinanceSplit.Client.Interfaces;
using FinanceSplit.Client.ViewModels;

namespace FinanceSplit.Client.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddClientDependencies(this IServiceCollection services)
    {
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<TransactionListViewModel>();
        services.AddTransient<TransactionViewModel>();
        services.AddTransient<PeopleListViewModel>();
        services.AddTransient<PersonViewModel>();

        services.AddTransient<ITransactionDialogService, TransactionDialogService>();
        services.AddTransient<IPersonDialogService, PersonDialogService>();

        services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
    }
}
