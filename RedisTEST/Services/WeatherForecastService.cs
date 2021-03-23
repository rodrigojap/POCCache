using MHCache.Extensions;
using MHCache.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RedisTEST.Services
{
    public class WeatherForecastService : IWeatherForecastService
    {
        private readonly string _fullNameClass = typeof(WeatherForecastService).FullName;
        
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly IResponseCacheService _responseCacheService;

        public WeatherForecastService(IResponseCacheService responseCacheService)
        {
            _responseCacheService = responseCacheService;
        }
      
        public async Task<bool> UpdateUser(int userId)
        {
            //SERVICE TO UPDATE USER
            //await repository.UpdateUser(userId);

            //REMOVE CACHE
            await _responseCacheService.RemoveAllByPattern($"{userId}:*");

            return true;
        }

        public async Task<WeatherForecast[]> GetWeatherForecasts()
        {
                                                                             //SPECIFIC ID:NAMESPACE:Method
            var result = await _responseCacheService.CacheResponseMethodAsync($"{GenerateRandomUserId()}:{_fullNameClass}:GetWeatherForecasts",
                                                                               null,
                                                                               () => GetWheater());

            return result;
        }

        public async Task<WeatherForecast> GetWeatherForecastById(int id)
        {
                                                                              //SPECIFIC ID:NAMESPACE:Method:Param
            var result = await _responseCacheService.CacheResponseMethodAsync($"{GenerateRandomUserId()}:{_fullNameClass}:GetWeatherForecastById:{id}",
                                                                                null,
                                                                                () => GetWheater().FirstOrDefault());

            return result;
        }

        private WeatherForecast[] GetWheater()
        {
            var rng = new Random();
            var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();

            return result;
        }

        private int GenerateRandomUserId()
        {
            var rng = new Random();
            var id = rng.Next(1000, 1005);
            return id;
        }      
    }
}
