using FinanceSplit.Contracts.Dashboard.DTOs;

namespace FinanceSplit.Contracts.Dashboard.Services;

public interface IDashboardService
{
    Task<DashboardDto?> GetDashboardAsync(
        DateOnly? month = null,
        CancellationToken cancellationToken = default
    );
}
