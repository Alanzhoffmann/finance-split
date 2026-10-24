using FinanceSplit.Contracts.DTOs;

namespace FinanceSplit.Contracts.Services;

public interface IWeatherService
{
    Task<IReadOnlyList<WeatherForecast>?> GetWeatherForecastsAsync(
        CancellationToken cancellationToken = default
    );
}
