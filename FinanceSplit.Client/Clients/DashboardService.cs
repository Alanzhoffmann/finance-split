using System.Text.Json.Serialization;
using FinanceSplit.Contracts.Dashboard.DTOs;
using FinanceSplit.Contracts.Dashboard.Services;

namespace FinanceSplit.Client.Clients;

internal partial class DashboardService(HttpClient client) : IDashboardService
{
    public async Task<DashboardDto?> GetDashboardAsync(
        DateOnly? month = null,
        CancellationToken cancellationToken = default
    ) =>
        await client.GetFromJsonAsync(
            month.HasValue ? $"api/dashboard?month={month:yyyy-MM}" : "api/dashboard",
            DashboardSerializerContext.Default.DashboardDto,
            cancellationToken: cancellationToken
        );

    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(DashboardDto))]
    partial class DashboardSerializerContext : JsonSerializerContext;
}
