using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using FinanceSplit.Contracts.Goals.DTOs;
using FinanceSplit.Contracts.People.DTOs;
using FinanceSplit.Contracts.People.Services;
using FinanceSplit.Contracts.Transactions.Commands;
using FinanceSplit.Contracts.Transactions.DTOs;
using FinanceSplit.Contracts.Transactions.Enums;
using FinanceSplit.Contracts.Transactions.Services;
using Microsoft.AspNetCore.Components;

namespace FinanceSplit.Client.ViewModels;

public partial class TransactionViewModel(
    IPersonService personService,
    ITransactionService transactionService
) : ObservableObject
{
    public async Task InitAsync(TransactionDto? transaction = null)
    {
        People = await personService.GetPeopleAsync() ?? [];

        if (transaction is not null)
        {
            Id = transaction.Id;
            Name = transaction.Name;
            Month = transaction.Month;
            Amount = transaction.Amount;
            SplitType = transaction.SplitType;
            PaidByPerson = transaction.PaidBy;
            SharedWith = [.. transaction.SharedWith.Select(p => p.Person)];
        }
        else
        {
            SplitType = SplitType.None;
        }

        if (PaidByPerson == default && People.Count > 0)
        {
            PaidByPerson = People[0];
        }
    }

    public bool IsEditing => Id is not null;

    [PersistentState]
    public IReadOnlyList<PersonDto> People { get; set; } = [];

    public Guid? Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public DateOnly? Month { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public decimal Amount { get; set; }

    [ObservableProperty]
    public partial PersonDto PaidByPerson { get; set; }

    [ObservableProperty]
    public partial SplitType SplitType { get; set; }
    public IReadOnlyCollection<PersonDto> SharedWith { get; set; } = [];
    public GoalDto? Goal { get; set; } = null;

    public bool AllowMultiselect => SplitType is not SplitType.None;

    public CreateTransactionCommand CreateCommand
    {
        get
        {
            if (SplitType is SplitType.None && SharedWith.Count > 1)
            {
                SharedWith = [SharedWith.First()];
            }

            return new CreateTransactionCommand(
                Name ?? throw new NullReferenceException("Name is null"),
                Month ?? throw new NullReferenceException("Month is null"),
                Amount,
                PaidByPerson.Id,
                SplitType,
                [.. SharedWith.Select(p => p.Id)],
                Goal?.Id
            );
        }
    }

    public UpdateTransactionCommand UpdateCommand
    {
        get
        {
            var createCommand = CreateCommand;
            return new UpdateTransactionCommand(
                Id ?? throw new NullReferenceException("Id is null"),
                createCommand.Name,
                createCommand.Date,
                createCommand.Value,
                createCommand.PaidByPersonId,
                createCommand.SplitType,
                createCommand.SharedWithIds,
                createCommand.GoalId
            );
        }
    }

    public async Task<TransactionDto?> SaveTransactionAsync(
        CancellationToken cancellationToken = default
    ) =>
        !IsEditing
            ? await transactionService.CreateTransactionAsync(CreateCommand, cancellationToken)
            : await transactionService.UpdateTransactionAsync(UpdateCommand, cancellationToken);

    partial void OnSplitTypeChanged(SplitType value)
    {
        if (value is not SplitType.None && People.Count == 2)
        {
            SharedWith = [.. People];
        }
    }

    partial void OnPaidByPersonChanged(PersonDto value)
    {
        if (SplitType is SplitType.None)
        {
            SharedWith = [value];
        }
    }
}
