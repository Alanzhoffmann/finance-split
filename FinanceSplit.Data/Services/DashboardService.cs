using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;

namespace FinanceSplit.Data.Services;

internal class DashboardService : IDashboardService
{
    public async Task<DashboardResponse?> GetDashboardAsync(
        DateOnly? month = null,
        CancellationToken cancellationToken = default
    ) => new();
}
