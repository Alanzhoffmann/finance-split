using FinanceSplit.Application.Mapping;
using FinanceSplit.Contracts.Enums;
using FinanceSplit.Contracts.Responses;
using FinanceSplit.Domain.Entities;
using FinanceSplit.Domain.Repositories;
using FinanceSplit.Domain.ValueObjects;

namespace FinanceSplit.Application.Services;

public class PersonService(IPersonRepository personRepository)
{
    public async Task<PersonResponse> CreatePersonAsync(string name, CancellationToken ct = default)
    {
        var person = new Person(name);
        await personRepository.AddAsync(person, ct);
        await personRepository.SaveChangesAsync(ct);
        return person.ToResponse();
    }

    public async Task<PersonResponse?> GetPersonAsync(Guid id, CancellationToken ct = default)
    {
        var person = await personRepository.GetByIdAsync(id, ct);
        return person?.ToResponse();
    }

    public async Task<IReadOnlyList<PersonResponse>> GetAllPeopleAsync(CancellationToken ct = default)
    {
        var people = await personRepository.GetAllAsync(ct);
        return people.Select(p => p.ToResponse()).ToList();
    }

    public async Task<PersonResponse?> AddSalaryAsync(Guid personId, DateOnly date, decimal amount, CancellationToken ct = default)
    {
        var person = await personRepository.GetByIdAsync(personId, ct);
        if (person is null)
        {
            return null;
        }

        person.AddSalary(new Salary(date, amount));
        await personRepository.SaveChangesAsync(ct);
        return person.ToResponse();
    }

    public async Task<PersonResponse?> UpdateNameAsync(Guid personId, string name, CancellationToken ct = default)
    {
        var person = await personRepository.GetByIdAsync(personId, ct);
        if (person is null)
        {
            return null;
        }

        person.UpdateName(name);
        await personRepository.SaveChangesAsync(ct);
        return person.ToResponse();
    }
}
