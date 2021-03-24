using System.Threading.Tasks;
using MHCache.AspNetCore.Filters.AOP.Extensions;

namespace RedisTEST.Services
{
    public interface IWeatherForecastService
    {
        //[CachedAOP]
        Task<WeatherForecast[]> GetWeatherForecasts();

        //[CachedAOP]
        Task<WeatherForecast> GetWeatherForecastById(int id);

        //[CachedAOP]
        Task<bool> UpdateUser(int userId);
    }
}
