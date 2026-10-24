using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Services;

namespace FinanceSplit.Client.Clients;

public class WeatherService(HttpClient client) : IWeatherService
{
    public async Task<IReadOnlyList<WeatherForecast>?> GetWeatherForecastsAsync(
        CancellationToken cancellationToken = default
    ) =>
        await client.GetFromJsonAsync<IReadOnlyList<WeatherForecast>>(
            "api/weather",
            cancellationToken: cancellationToken
        );
}
