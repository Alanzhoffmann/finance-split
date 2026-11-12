using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FinanceSplit.Client.Interfaces;
using FinanceSplit.Client.ViewModels.Messages;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Components;

namespace FinanceSplit.Client.ViewModels;

public partial class PeopleListViewModel(
    IPersonService personService,
    IPersonDialogService dialogService,
    IMessenger messenger
) : ObservableRecipient(messenger), IRecipient<PersonDeletedMessage>
{
    [PersistentState(AllowUpdates = true)]
    public List<PersonDto>? People { get; private set; }

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

    public void Receive(PersonDeletedMessage message)
    {
        if (IsLoaded)
        {
            var index = People.FindIndex(p => p.Id == message.Id);
            if (index != -1)
            {
                People.RemoveAt(index);
            }
        }
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
