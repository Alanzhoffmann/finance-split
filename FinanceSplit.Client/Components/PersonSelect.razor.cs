using FinanceSplit.Contracts.People.DTOs;
using FinanceSplit.Contracts.People.Services;
using Microsoft.AspNetCore.Components;

namespace FinanceSplit.Client.Components;

public partial class PersonSelect(IPersonService personService)
{
    [Parameter]
    public string Label { get; set; } = "Person";

    [Parameter]
    public string? HelperText { get; set; }

    [Parameter]
    public bool MultiSelection { get; set; }

    [Parameter]
    public PersonListDto? Value { get; set; }

    [Parameter]
    public EventCallback<PersonListDto> ValueChanged { get; set; }

    [Parameter]
    public IReadOnlyCollection<PersonListDto>? SelectedValues { get; set; }

    [Parameter]
    public EventCallback<IReadOnlyCollection<PersonListDto>> SelectedValuesChanged { get; set; }

    [PersistentState]
    public IReadOnlyList<PersonListDto>? People { get; set; }

    protected override async Task OnInitializedAsync()
    {
        People ??= await personService.GetPeopleListAsync() ?? [];
    }
}
