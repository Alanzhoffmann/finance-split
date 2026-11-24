using CommunityToolkit.Mvvm.Messaging;
using FinanceSplit.Client.Dialogs.Services;
using FinanceSplit.Client.Interfaces;
using FinanceSplit.Client.ViewModels;
using FinanceSplit.Client.ViewModels.Factories;

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
        services.AddTransient<IncomeRecordViewModel>();
        services.AddTransient<GoalListViewModel>();

        services.AddTransient<GoalViewModelFactory>();

        services.AddTransient<ITransactionDialogService, TransactionDialogService>();
        services.AddTransient<IPersonDialogService, PersonDialogService>();
        services.AddTransient<IGoalDialogService, GoalDialogService>();

        services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
    }
}
