using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Components;

namespace FinanceSplit.Client.Pages;

public partial class PeopleList(IPersonService personService)
{
    [PersistentState(AllowUpdates = true)]
    public IReadOnlyList<PersonResponse>? People { get; set; }

    protected override async Task OnInitializedAsync()
    {
        People = await personService.GetPeopleAsync();
    }
}
