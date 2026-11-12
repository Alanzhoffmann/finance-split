using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using FinanceSplit.Contracts.Commands;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Enums;
using FinanceSplit.Contracts.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FinanceSplit.Client.ViewModels;

public partial class TransactionViewModel(
    IPersonService personService,
    ITransactionService transactionService
) : ObservableObject
{
    public static IReadOnlyList<SplitType> SplitTypes => Enum.GetValues<SplitType>();

    public async Task InitAsync(TransactionDto? transaction = null)
    {
        People = await personService.GetPeopleAsync() ?? [];

        if (transaction is not null)
        {
            Id = transaction.Id;
            Name = transaction.Name;
            Date = transaction.Date.ToDateTime(TimeOnly.MinValue);
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
    public IReadOnlyList<PersonResponse> People { get; set; } = [];
    public Guid? Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public DateTime? Date { get; set; } = DateTime.Today;
    public decimal Value { get; set; }

    [ObservableProperty]
    public partial Guid PaidByPersonId { get; set; }

    [ObservableProperty]
    public partial SplitType SplitType { get; set; }
    public IReadOnlyCollection<Guid> SharedWithIds { get; set; } = [];

    public bool AllowMultiselect => SplitType is not SplitType.None;

    public CreateTransactionCommand CreateCommand
    {
        get
        {
            var name = Name ?? throw new NullReferenceException("Name is null");
            var date = Date is null
                ? throw new NullReferenceException("Date is null")
                : DateOnly.FromDateTime(Date.Value);

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
                SharedWithIds
            );
        }
    }

    public UpdateTransactionCommand UpdateCommand
    {
        get
        {
            if (!Id.HasValue)
            {
                throw new NullReferenceException("Id is null");
            }

            var createCommand = CreateCommand;
            return new UpdateTransactionCommand(
                Id.Value,
                createCommand.Name,
                createCommand.Date,
                createCommand.Value,
                createCommand.PaidByPersonId,
                createCommand.SplitType,
                createCommand.SharedWithIds
            );
        }
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
            ? await transactionService.CreateTransactionAsync(CreateCommand, cancellationToken)
            : await transactionService.UpdateTransactionAsync(UpdateCommand, cancellationToken);

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
