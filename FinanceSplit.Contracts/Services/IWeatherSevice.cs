using FinanceSplit.Contracts.DTOs;

namespace FinanceSplit.Contracts.Services;

public interface IWeatherSevice
{
    Task<IReadOnlyList<WeatherForecast>?> GetWeatherForecastsAsync(CancellationToken cancellationToken = default);
}
