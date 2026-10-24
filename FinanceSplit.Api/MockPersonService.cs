using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Enums;
using FinanceSplit.Contracts.Services;

namespace FinanceSplit.Api;

internal class MockPersonService : IPersonService
{
    public async Task<IReadOnlyList<PersonResponse>?> GetPeopleAsync(
        CancellationToken cancellationToken = default
    ) =>
        [
            new PersonResponse(
                Guid.NewGuid(),
                "name 1",
                [
                    new IncomeRecordResponse(
                        Guid.NewGuid(),
                        new DateTime(2026, 10, 1),
                        2000,
                        IncomeType.Salary
                    ),
                    new IncomeRecordResponse(
                        Guid.NewGuid(),
                        new DateTime(2026, 9, 1),
                        2000,
                        IncomeType.Salary
                    ),
                    new IncomeRecordResponse(
                        Guid.NewGuid(),
                        new DateTime(2026, 9, 1),
                        500,
                        IncomeType.Extra
                    ),
                ]
            ),
            new PersonResponse(
                Guid.NewGuid(),
                "name 2",
                [
                    new IncomeRecordResponse(
                        Guid.NewGuid(),
                        new DateTime(2026, 10, 1),
                        2000,
                        IncomeType.Salary
                    ),
                ]
            ),
        ];
}
