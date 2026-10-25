using FinanceSplit.Contracts.Commands;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;
using FinanceSplit.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceSplit.Data.Services;

internal class PersonService(AppDbContext context) : IPersonService
{
    public async Task<IReadOnlyList<PersonResponse>?> GetPeopleAsync(
        CancellationToken cancellationToken = default
    ) =>
        await context
            .People.OrderBy(p => p.Name)
            .Select(p => new PersonResponse(
                p.Id,
                p.Name,
                p.IncomeRecords.OrderBy(i => i.Month)
                    .ThenBy(i => i.Type)
                    .Select(i => new IncomeRecordResponse(i.Id, i.Month, i.Amount, i.Type))
                    .ToList()
            ))
            .ToListAsync(cancellationToken: cancellationToken);

    public async Task<PersonResponse?> CreatePersonAsync(
        CreatePersonCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var person = new Person { Name = command.Name };

        context.Add(person);

        await context.SaveChangesAsync(cancellationToken);

        return new PersonResponse(person.Id, person.Name, []);
    }

    public async Task<PersonResponse?> UpdatePersonAsync(
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

        return new PersonResponse(person.Id, person.Name, []);
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
}
