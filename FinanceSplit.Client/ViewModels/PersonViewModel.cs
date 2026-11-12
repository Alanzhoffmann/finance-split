using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FinanceSplit.Client.Interfaces;
using FinanceSplit.Client.ViewModels.Messages;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;

namespace FinanceSplit.Client.ViewModels;

public partial class PersonViewModel(
    IPersonService personService,
    IPersonDialogService dialogService,
    IMessenger messenger
)
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public List<IncomeRecordResponse> Incomes { get; private set; } = [];

    private PersonDto Person => new(Id, Name) { Incomes = Incomes };

    public void Initialize(PersonDto person)
    {
        Id = person.Id;
        Name = person.Name;
        Incomes = [.. person.Incomes];
    }

    [RelayCommand]
    private async Task EditPersonAsync()
    {
        var editedPerson = await dialogService.EditPersonAsync(Person);
        if (editedPerson is not null)
        {
            Name = editedPerson.Name;
        }
    }

    [RelayCommand]
    private async Task DeletePersonAsync()
    {
        var result = await dialogService.ShowDeletePersonConfirmationAsync(Person);
        if (result == true)
        {
            await personService.DeletePersonAsync(new(Id));
            messenger.Send(new PersonDeletedMessage(Id));
        }
    }

    [RelayCommand]
    private async Task CreateIncomeRecordAsync()
    {
        var newIncome = await dialogService.CreateIncomeAsync(Person);
        if (newIncome is not null)
        {
            Incomes.Add(newIncome);
            Incomes.Sort((x, y) => x.Month.CompareTo(y.Month));
        }
    }

    [RelayCommand]
    private async Task EditIncomeRecordAsync(IncomeRecordResponse incomeRecord)
    {
        var editedIncome = await dialogService.EditIncomeAsync(Person, incomeRecord);
        if (editedIncome is not null)
        {
            var index = Incomes.FindIndex(i => i.Id == editedIncome.Id);
            if (index != -1)
            {
                Incomes[index] = editedIncome;
            }
        }
    }

    [RelayCommand]
    private async Task DeleteIncomeRecordAsync(IncomeRecordResponse incomeRecord)
    {
        var result = await dialogService.ShowDeleteIncomeConfirmationAsync(Person, incomeRecord);
        if (result == true)
        {
            await personService.DeleteIncomeRecordAsync(new(incomeRecord.Id, Id));
            var index = Incomes.FindIndex(i => i.Id == incomeRecord.Id);
            if (index != -1)
            {
                Incomes.RemoveAt(index);
            }
        }
    }
}
