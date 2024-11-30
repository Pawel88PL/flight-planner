using backend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherApiHelper _weatherApiHelper;

        public WeatherController(IWeatherApiHelper weatherApiHelper)
        {
            _weatherApiHelper = weatherApiHelper;
        }

        [HttpGet("{departureICAO},{arrivalICAO}")]
        public async Task<IActionResult> GetWeatherData(string departureICAO, string arrivalICAO)
        {
            try
            {
                var weather = await _weatherApiHelper.GetAsync<object>(departureICAO, arrivalICAO);
                return Ok(weather);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}