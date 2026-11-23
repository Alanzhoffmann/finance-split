using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using FinanceSplit.Client.Interfaces;
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
    ITransactionService transactionService,
    ITransactionDialogService dialogService
) : ObservableObject
{
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
            if (transaction.Recurrence is not null)
            {
                IsRecurring = true;
                RecurrenceId = transaction.Recurrence.Id;
                RecurrenceType = transaction.Recurrence.Type;
                NumberOfRecurrences = transaction.Recurrence.NumberOfRecurrences ?? 0;
                LastMonth = transaction.Recurrence.LastMonth is null
                    ? null
                    : new DateTime(transaction.Recurrence.LastMonth.Value, TimeOnly.MinValue);
            }
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

    public static IReadOnlyList<(RecurrenceType Value, string Name)> RecurrenceTypes { get; } =
        Enum.GetValues<RecurrenceType>()
            .Select(r => (r, r.ToStringFast(useMetadataAttributes: true)))
            .ToList();
    public static IReadOnlyList<(SplitType Value, string Name)> SplitTypes { get; } =
        Enum.GetValues<SplitType>()
            .Select(s => (s, s.ToStringFast(useMetadataAttributes: true)))
            .ToList();

    public bool IsUpdate => Id is not null;

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

    public Guid? RecurrenceId { get; set; }

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

        return new CreateTransactionCommand(
            name,
            date,
            Value,
            PaidByPersonId,
            SplitType,
            SharedWithIds,
            RecurrenceSetup: GetRecurrenceSetup()
        );
    }

    private RecurrenceTransactionSetup? GetRecurrenceSetup()
    {
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

        return recurrenceSetup;
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
            createCommand.Month,
            createCommand.Value,
            createCommand.PaidByPersonId,
            createCommand.SplitType,
            createCommand.SharedWithIds
        );
    }

    public async Task<UpdateRecurringTransactionCommand?> GetUpdateRecurringTransactionCommandAsync()
    {
        if (!Id.HasValue)
        {
            throw new NullReferenceException("Id is null");
        }

        var updateCommand = GetUpdateCommand();

        var updateRecurrence = await dialogService.GetRecurrenceUpdateTypeAsync();
        if (updateRecurrence is null)
        {
            return null;
        }

        return new UpdateRecurringTransactionCommand(
            RecurrenceId.Value,
            Id.Value,
            updateCommand.Name,
            updateCommand.Month,
            updateCommand.Value,
            updateCommand.PaidByPersonId,
            updateCommand.SplitType,
            updateCommand.SharedWithIds,
            GetRecurrenceSetup() with
            {
                UpdateRecurrence = updateRecurrence,
            }
        );
    }

    public string GetMultiSelectionText(IReadOnlyList<string> selectedValues) =>
        string.Join(
            ", ",
            People.Where(p => selectedValues.Contains(p.Id.ToString())).Select(p => p.Name).Order()
        );

    public async Task<TransactionDto?> SaveTransactionAsync(
        CancellationToken cancellationToken = default
    )
    {
        if (!IsUpdate)
        {
            return await transactionService.CreateTransactionAsync(
                GetCreateCommand(),
                cancellationToken
            );
        }
        else if (RecurrenceId is null)
        {
            return await transactionService.UpdateTransactionAsync(
                GetUpdateCommand(),
                cancellationToken
            );
        }
        else
        {
            var command = await GetUpdateRecurringTransactionCommandAsync();
            if (command is null)
            {
                return null;
            }

            return await transactionService.UpdateRecurringTransactionAsync(
                command,
                cancellationToken
            );
        }
    }

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
