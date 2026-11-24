using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using FinanceSplit.Contracts.People.Commands;
using FinanceSplit.Contracts.People.DTOs;
using FinanceSplit.Contracts.People.Enums;
using FinanceSplit.Contracts.People.Services;

namespace FinanceSplit.Client.ViewModels;

public partial class IncomeRecordViewModel(IPersonService personService) : ObservableValidator
{
    public void Initialize(PersonDto person, IncomeRecordDto? incomeRecord = null)
    {
        PersonId = person.Id;

        if (incomeRecord is not null)
        {
            Id = incomeRecord.Id;
            Amount = incomeRecord.Amount;
            Month = incomeRecord.Month;
            IncomeType = incomeRecord.Type;
        }
    }

    public Guid PersonId { get; set; }

    public Guid? Id { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public DateOnly? Month { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public IncomeType IncomeType { get; set; } = IncomeType.Salary;

    public bool IsEditing => Id is not null;

    [ObservableProperty]
    public partial bool IsSaving { get; private set; }

    public async Task<IncomeRecordDto?> SaveIncomeAsync(
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            IsSaving = true;

            return IsEditing
                ? await personService.UpdateIncomeRecordAsync(GetUpdateCommand(), cancellationToken)
                : await personService.CreateIncomeRecordAsync(
                    GetCreateCommand(),
                    cancellationToken
                );
        }
        finally
        {
            IsSaving = false;
        }
    }

    public CreateIncomeRecordCommand GetCreateCommand()
    {
        if (Month is null)
        {
            throw new ArgumentNullException(nameof(Month));
        }

        var month = Month.Value;
        return new CreateIncomeRecordCommand(PersonId, Amount, month, IncomeType);
    }

    public UpdateIncomeRecordCommand GetUpdateCommand()
    {
        if (!Id.HasValue)
        {
            throw new ArgumentNullException(nameof(Id));
        }

        if (Month is null)
        {
            throw new ArgumentNullException(nameof(Month));
        }

        var month = Month.Value;
        return new UpdateIncomeRecordCommand(Id.Value, PersonId, Amount, month, IncomeType);
    }
}
