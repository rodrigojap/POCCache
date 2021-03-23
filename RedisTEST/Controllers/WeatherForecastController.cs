using System;
using System.Linq;
using System.Threading.Tasks;
using MHCache.AspNetCore.Filters;
using Microsoft.AspNetCore.Mvc;
using RedisTEST.Services;

namespace RedisTEST.Controllers
{
    //[TypeFilter(typeof(CachedAttribute), Arguments = new object[] { 360 })]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IWeatherForecastService _weatherForecastService;

        public WeatherForecastController(IWeatherForecastService weatherForecastService)
        {
            _weatherForecastService = weatherForecastService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _weatherForecastService.GetWeatherForecasts());
        }

        [HttpGet("{id}")]        
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            return Ok(await _weatherForecastService.GetWeatherForecastById(id));
        }


        [HttpPut("{userId}")]
        public async Task<IActionResult> InvalidateCacheByUser([FromRoute] int userId)
        {            
            //chamar via rabbit após atualização da entidade
            await _weatherForecastService.InvalidateCachePattern(userId.ToString());

            return Ok();
        }
    }
}
