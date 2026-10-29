using System.Text.Json.Serialization;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;

namespace FinanceSplit.Client.Clients;

internal partial class DashboardService(HttpClient client) : IDashboardService
{
    public async Task<DashboardResponse?> GetDashboardAsync(
        DateOnly? month = null,
        CancellationToken cancellationToken = default
    ) =>
        await client.GetFromJsonAsync(
            month.HasValue ? $"api/dashboard?month={month}" : "api/dashboard",
            DashboardSerializerContext.Default.DashboardResponse,
            cancellationToken: cancellationToken
        );

    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(DashboardResponse))]
    partial class DashboardSerializerContext : JsonSerializerContext;
}
