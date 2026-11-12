using FinanceSplit.Contracts.Commands;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;
using FinanceSplit.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceSplit.Data.Services;

internal class PersonService(AppDbContext context) : IPersonService
{
    public async Task<IReadOnlyList<PersonDto>?> GetPeopleAsync(
        CancellationToken cancellationToken = default
    ) =>
        await context
            .People.OrderBy(p => p.Name)
            .Select(p => new PersonDto(p.Id, p.Name)
            {
                Incomes = p
                    .IncomeRecords.OrderBy(i => i.Month)
                    .ThenBy(i => i.Type)
                    .Select(i => new IncomeRecordResponse(i.Id, i.Month, i.Amount, i.Type))
                    .ToList(),
            })
            .ToListAsync(cancellationToken: cancellationToken);

    public async Task<PersonDto?> CreatePersonAsync(
        CreatePersonCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var person = new Person { Name = command.Name };

        context.Add(person);

        await context.SaveChangesAsync(cancellationToken);

        return new PersonDto(person.Id, person.Name);
    }

    public async Task<PersonDto?> UpdatePersonAsync(
        UpdatePersonCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var person = await context.People.FindAsync([command.Id], cancellationToken);

        if (person is null)
        {
            return null;
        }

        person.UpdateName(command.Name);

        await context.SaveChangesAsync(cancellationToken);

        return new PersonDto(person.Id, person.Name);
    }

    public async Task DeletePersonAsync(
        DeletePersonCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var personToDelete = await context.People.FindAsync([command.Id], cancellationToken);
        if (personToDelete is null)
        {
            // TODO figure out 404s here
            return;
        }

        context.Remove(personToDelete);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IncomeRecordResponse?> CreateIncomeRecordAsync(
        CreateIncomeRecordCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var person = await context.People.FindAsync([command.PersonId], cancellationToken);
        if (person is null)
        {
            return null;
        }

        var newIncome = IncomeRecord.ForMonth(
            command.Month.Month,
            command.Month.Year,
            command.Amount,
            command.Type
        );
        person.IncomeRecords.Add(newIncome);

        await context.SaveChangesAsync(cancellationToken);

        return new(newIncome.Id, newIncome.Month, newIncome.Amount, newIncome.Type);
    }

    public async Task<IncomeRecordResponse?> UpdateIncomeRecordAsync(
        UpdateIncomeRecordCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var person = await context
            .People.Include(p => p.IncomeRecords.Where(i => i.Id == command.Id))
            .FirstOrDefaultAsync(p => p.Id == command.PersonId, cancellationToken);
        if (person is null)
        {
            return null;
        }

        var incomeRecord = person.IncomeRecords.FirstOrDefault();
        if (incomeRecord is null)
        {
            return null;
        }

        incomeRecord.Update(command.Amount, command.Month, command.Type);

        await context.SaveChangesAsync(cancellationToken);

        return new(incomeRecord.Id, incomeRecord.Month, incomeRecord.Amount, incomeRecord.Type);
    }

    public async Task DeleteIncomeRecordAsync(
        DeleteIncomeRecordCommand command,
        CancellationToken cancellationToken = default
    )
    {
        // TODO improve this by calling person first and editing the list there
        var incomeRecordToDelete = await context.IncomeRecords.FindAsync(
            [command.Id],
            cancellationToken
        );
        if (incomeRecordToDelete is null)
        {
            // TODO figure out 404s here
            return;
        }

        context.Remove(incomeRecordToDelete);

        await context.SaveChangesAsync(cancellationToken);
    }
}
