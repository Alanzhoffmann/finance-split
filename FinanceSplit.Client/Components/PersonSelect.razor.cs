using FinanceSplit.Contracts.People.DTOs;
using FinanceSplit.Contracts.People.Services;
using Microsoft.AspNetCore.Components;

namespace FinanceSplit.Client.Components;

public partial class PersonSelect(IPersonService personService)
{
    [Parameter]
    public string? Label { get; set; }

    [Parameter]
    public string? HelperText { get; set; }

    [Parameter]
    public bool MultiSelection { get; set; }

    [Parameter]
    public Guid Value { get; set; }

    [Parameter]
    public EventCallback<Guid> ValueChanged { get; set; }

    [Parameter]
    public IReadOnlyCollection<Guid>? SelectedValues { get; set; }

    [Parameter]
    public EventCallback<IReadOnlyCollection<Guid>?> SelectedValuesChanged { get; set; }

    [PersistentState]
    public IReadOnlyList<PersonDto>? People { get; set; }

    protected override async Task OnInitializedAsync()
    {
        People = await personService.GetPeopleAsync() ?? [];
    }

    private string GetMultiSelectionText(IReadOnlyList<string> selectedValues) =>
        string.Join(
            ", ",
            (People ?? [])
                .Where(p => selectedValues.Contains(p.Id.ToString()))
                .Select(p => p.Name)
                .Order()
        );
}
