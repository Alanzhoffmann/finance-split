using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using FinanceSplit.Contracts.Commands;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Enums;
using FinanceSplit.Contracts.Services;
using MudBlazor;

namespace FinanceSplit.Client.ViewModels;

public partial class TransactionModel(
    IPersonService personService,
    ITransactionService transactionService
) : ObservableObject
{
    public async Task InitAsync(TransactionResponse? transaction = null)
    {
        People = await personService.GetPeopleAsync() ?? [];

        if (transaction is not null)
        {
            Id = transaction.Id;
            Name = transaction.Name;
            Date = transaction.Date.ToDateTime(TimeOnly.MinValue);
            Value = transaction.Value;
            PaidByPersonId = transaction.PaidBy.Id;
            IsRecurring = transaction.IsRecurring;
            SplitType = transaction.SplitType;
            SharedWithIds = [.. transaction.SharedWith.Select(p => p.Id)];
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

    public IReadOnlyList<PersonResponse> People { get; set; } = [];
    public Guid? Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public DateTime? Date { get; set; } = DateTime.Today;
    public decimal Value { get; set; }
    public Guid PaidByPersonId { get; set; }
    public bool IsRecurring { get; set; }

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

            Guid[] sharedWith = [];
            if (SplitType is not SplitType.None)
            {
                sharedWith = [.. SharedWithIds];
            }

            return new CreateTransactionCommand(
                name,
                date,
                Value,
                PaidByPersonId,
                IsRecurring,
                SplitType,
                sharedWith
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
                createCommand.IsRecurring,
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

    public async Task<TransactionResponse?> SaveTransactionAsync(
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
}
