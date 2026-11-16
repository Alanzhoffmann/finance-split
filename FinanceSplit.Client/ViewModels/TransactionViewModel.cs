using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using FinanceSplit.Contracts.People.DTOs;
using FinanceSplit.Contracts.People.Services;
using FinanceSplit.Contracts.Transactions.Commands;
using FinanceSplit.Contracts.Transactions.DTOs;
using FinanceSplit.Contracts.Transactions.Enums;
using FinanceSplit.Contracts.Transactions.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FinanceSplit.Client.ViewModels;

public partial class TransactionViewModel(
    IPersonService personService,
    ITransactionService transactionService
) : ObservableObject
{
    public static IReadOnlyList<SplitType> SplitTypes => Enum.GetValues<SplitType>();
    public static IReadOnlyList<RecurrenceType> RecurrenceTypes => Enum.GetValues<RecurrenceType>();

    public async Task InitAsync(TransactionDto? transaction = null)
    {
        People = await personService.GetPeopleAsync() ?? [];

        if (transaction is not null)
        {
            Id = transaction.Id;
            Name = transaction.Name;
            Month = transaction.Month.ToDateTime(TimeOnly.MinValue);
            Value = transaction.Value;
            SplitType = transaction.SplitType;
            PaidByPersonId = transaction.PaidBy.Id;
            SharedWithIds = [.. transaction.SharedWith.Select(p => p.Person.Id)];
        }
        else
        {
            SplitType = SplitType.None;
        }

        if (PaidByPersonId == default && People.Count > 0)
        {
            PaidByPersonId = People[0].Id;
        }
    }

    public bool IsEditing => Id is not null;

    [PersistentState]
    public IReadOnlyList<PersonDto> People { get; set; } = [];
    public Guid? Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public DateTime? Month { get; set; } = DateTime.Today;
    public decimal Value { get; set; }

    [ObservableProperty]
    public partial Guid PaidByPersonId { get; set; }

    [ObservableProperty]
    public partial SplitType SplitType { get; set; }
    public IReadOnlyCollection<Guid> SharedWithIds { get; set; } = [];

    public bool AllowMultiselect => SplitType is not SplitType.None;

    public bool IsRecurring { get; set; }

    public RecurrenceType RecurrenceType { get; set; }

    public bool IsRecurringPeriodOfTime => RecurrenceType is RecurrenceType.PeriodOfTime;
    public bool IsRecurringNumberOfTimes => RecurrenceType is RecurrenceType.NumberOfTimes;

    public int NumberOfRecurrences { get; set; }
    public DateTime? LastMonth { get; set; }
    public bool IsSplit { get; set; }

    public CreateTransactionCommand GetCreateCommand()
    {
        var name = Name ?? throw new NullReferenceException("Name is null");
        var date = Month is null
            ? throw new NullReferenceException("Date is null")
            : DateOnly.FromDateTime(Month.Value);

        if (SplitType is SplitType.None && SharedWithIds.Count > 1)
        {
            SharedWithIds = [SharedWithIds.First()];
        }

        RecurrenceTransactionSetup? recurrenceSetup = null;
        if (IsRecurring)
        {
            recurrenceSetup = new RecurrenceTransactionSetup(
                IsRecurringNumberOfTimes ? NumberOfRecurrences : null,
                IsRecurringPeriodOfTime && LastMonth.HasValue
                    ? DateOnly.FromDateTime(LastMonth.Value)
                    : null
            );
        }

        return new CreateTransactionCommand(
            name,
            date,
            Value,
            PaidByPersonId,
            SplitType,
            SharedWithIds,
            RecurrenceSetup: recurrenceSetup
        );
    }

    public UpdateTransactionCommand GetUpdateCommand()
    {
        if (!Id.HasValue)
        {
            throw new NullReferenceException("Id is null");
        }

        var createCommand = GetCreateCommand();
        return new UpdateTransactionCommand(
            Id.Value,
            createCommand.Name,
            createCommand.Date,
            createCommand.Value,
            createCommand.PaidByPersonId,
            createCommand.SplitType,
            createCommand.SharedWithIds,
            createCommand.SplitSetup,
            createCommand.RecurrenceSetup
        );
    }

    public string GetMultiSelectionText(IReadOnlyList<string> selectedValues) =>
        string.Join(
            ", ",
            People.Where(p => selectedValues.Contains(p.Id.ToString())).Select(p => p.Name).Order()
        );

    public async Task<TransactionDto?> SaveTransactionAsync(
        CancellationToken cancellationToken = default
    ) =>
        !IsEditing
            ? await transactionService.CreateTransactionAsync(GetCreateCommand(), cancellationToken)
            : await transactionService.UpdateTransactionAsync(
                GetUpdateCommand(),
                cancellationToken
            );

    partial void OnSplitTypeChanged(SplitType value)
    {
        if (value is not SplitType.None && People.Count == 2)
        {
            SharedWithIds = [.. People.Select(p => p.Id)];
        }
    }

    partial void OnPaidByPersonIdChanged(Guid value)
    {
        if (SplitType is SplitType.None)
        {
            SharedWithIds = [value];
        }
    }
}
