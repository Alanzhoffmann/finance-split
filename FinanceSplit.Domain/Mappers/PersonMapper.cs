using FinanceSplit.Contracts.People.DTOs;
using FinanceSplit.Domain.Entities;

namespace FinanceSplit.Domain.Mappers;

public static class PersonMapper
{
    public static PersonDto ToDto(this Person person) => new(person.Id, person.Name);

    public static PersonListDto ToListDto(this Person person) => new(person.Id, person.Name);
}
