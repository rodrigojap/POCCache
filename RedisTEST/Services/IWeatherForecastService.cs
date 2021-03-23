using System.Threading.Tasks;

namespace RedisTEST.Services
{
    public interface IWeatherForecastService
    {
        Task<WeatherForecast[]> GetWeatherForecasts();

        Task<WeatherForecast> GetWeatherForecastById(int id);

        Task<bool> UpdateUser(int userId);
    }
}
