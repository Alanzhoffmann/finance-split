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
            .People.Select(p => new PersonResponse(
                p.Id,
                p.Name,
                p.IncomeRecords.Select(i => new IncomeRecordResponse(
                        i.Id,
                        i.Month,
                        i.Amount,
                        i.Type
                    ))
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
}
