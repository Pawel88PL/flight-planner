using backend.Interfaces;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AircraftController : ControllerBase
    {
        private readonly IAircraftService _aircraftService;

        public AircraftController(IAircraftService aircraftService)
        {
            _aircraftService = aircraftService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddAircraft(Aircraft aircraft)
        {
            try
            {
                await _aircraftService.AddAircraft(aircraft);
                return Ok();
            }
            catch (Exception e)
            {
                var message = "Wystąpił błąd podczas dodawania samolotu" + e.Message;
                Log.Error(message);
                return BadRequest(new { message });
            }
        }
    }
}