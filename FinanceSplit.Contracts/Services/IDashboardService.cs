using FinanceSplit.Contracts.DTOs;

namespace FinanceSplit.Contracts.Services;

public interface IDashboardService
{
    Task<DashboardResponse?> GetDashboardAsync(
        DateOnly? month = null,
        CancellationToken cancellationToken = default
    );
}
