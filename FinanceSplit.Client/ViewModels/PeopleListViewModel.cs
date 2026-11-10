using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.Input;
using FinanceSplit.Client.Interfaces;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Components;

namespace FinanceSplit.Client.ViewModels;

public partial class PeopleListViewModel(
    IPersonService personService,
    IPersonDialogService dialogService
)
{
    [PersistentState(AllowUpdates = true)]
    public List<PersonResponse>? People { get; private set; }

    [MemberNotNullWhen(true, nameof(People))]
    public bool IsLoaded => People is not null;

    public async Task InitializeAsync()
    {
        if (IsLoaded)
        {
            return;
        }

        var data = await personService.GetPeopleAsync();
        if (data is null)
        {
            return;
        }

        People = [.. data];
    }

    [RelayCommand]
    private async Task CreatePerson()
    {
        var newPerson = await dialogService.CreatePersonAsync();
        if (newPerson is not null)
        {
            if (IsLoaded)
            {
                People.Add(newPerson);
                People.Sort((x, y) => x.Name.CompareTo(y.Name));
            }
        }
    }
}
