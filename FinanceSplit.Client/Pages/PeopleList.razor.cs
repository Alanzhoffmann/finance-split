using FinanceSplit.Client.ViewModels;

namespace FinanceSplit.Client.Pages;

public partial class PeopleList(PeopleListViewModel viewModel)
{
    private PeopleListViewModel ViewModel { get; } = viewModel;

    protected override async Task OnInitializedAsync()
    {
        await ViewModel.InitializeAsync();
    }
}
