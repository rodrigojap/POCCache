using System;
using System.Linq;
using MHCache.AspNetCore.Filters;
using Microsoft.AspNetCore.Mvc;

namespace RedisTEST.Controllers
{
    [TypeFilter(typeof(CachedAttribute), Arguments = new object[] { 360 })]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public WeatherForecastController()
        {
        }

        [HttpGet("{id}")]
        public IActionResult Get([FromRoute]int id)
        {                        
            var rng = new Random();
            var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();

            return Ok(result);
        }

        [HttpPost()]
        public IActionResult Post()
        {
            return Created("/WeatherForecast/1",null);
        }
    }
}
