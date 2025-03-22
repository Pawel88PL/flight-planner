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
        [HttpPost("add")]
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAircraftById(int id)
        {
            try
            {
                var aircraft = await _aircraftService.GetAircraftById(id);

                if (aircraft == null)
                {
                    return NotFound();
                }

                return Ok(aircraft);
            }
            catch (Exception e)
            {
                var message = $"Wystąpił błąd podczas pobierania samolotu: {e.Message}";
                Log.Error(message);
                return BadRequest(new { message });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAircrafts()
        {
            try
            {
                var aircrafts = await _aircraftService.GetAircrafts();
                return Ok(aircrafts);
            }
            catch (Exception e)
            {
                var message = $"Wystąpił błąd podczas pobierania samolotów: {e.Message}";
                Log.Error(message);
                return BadRequest(new { message });
            }
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetAircraftsPaged([FromQuery] PagedRequest pagedRequest)
        {
            try
            {
                var aircrafts = await _aircraftService.GetAircraftsPaged(pagedRequest);
                return Ok(aircrafts);
            }
            catch (Exception e)
            {
                var message = $"Wystąpił błąd podczas pobierania samolotów: {e.Message}";
                Log.Error(message);
                return BadRequest(new { message });
            }
        }
    }
}