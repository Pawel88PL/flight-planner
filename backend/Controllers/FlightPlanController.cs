using backend.Interfaces;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/flight-plan")]
    public class FlightPlanController : Controller
    {
        private readonly IFlightPlanService _flightPlanService;

        public FlightPlanController(IFlightPlanService flightPlanService)
        {
            _flightPlanService = flightPlanService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateFlightPlanAsync(FlightPlanRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var responseId = await _flightPlanService.CreateFlightPlan(request);
                return Ok(new { responseId });
            }
            catch (Exception ex)
            {
                var message = "Wystąpił błąd podczas dodawania nowego zapytania o plan lotu. " + ex.Message;
                Log.Error(message);
                return BadRequest(new { message });
            }
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetFlightPlanResponseAsync(int id)
        {
            try
            {
                var flightPlan = await _flightPlanService.GetFlightPlan(id);
                return Ok(flightPlan);
            }
            catch (Exception ex)
            {
                var message = "Wystąpił błąd podczas pobierania zapytania o plan lotu. " + ex.Message;
                Log.Error(message);
                return BadRequest(new { message });
            }
        }
    }
}