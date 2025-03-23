using System.Security.Claims;
using backend.Interfaces;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "Admin, Pilot")]
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
                var message = ex.Message;
                Log.Error(message);
                return BadRequest(new { message });
            }
        }

        [Authorize(Roles = "Pilot, Admin")]
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

        [Authorize(Roles = "Pilot, Admin")]
        [HttpGet("get-flight-plans-by-userId")]
        public async Task<IActionResult> GetFlightPlansForUserAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { message = "Nie znaleziono identyfikatora użytkownika." });
            }

            try
            {
                var flightPlans = await _flightPlanService.GetFlightPlansForUser(userId);
                return Ok(flightPlans);
            }
            catch (Exception ex)
            {
                var message = "Wystąpił błąd podczas pobierania planów lotu dla użytkownika. " + ex.Message;
                Log.Error(message);
                return BadRequest(new { message });
            }
        }

        [Authorize(Roles = "Pilot, Admin")]
        [HttpGet("paged")]
        public async Task<IActionResult> GetFlightPlansPagedAsync([FromQuery] PagedRequest request)
        {
            try
            {
                var flightPlans = await _flightPlanService.GetFlightPlansPaged(request);
                return Ok(flightPlans);
            }
            catch (Exception ex)
            {
                var message = "Wystąpił błąd podczas pobierania planów lotu. " + ex.Message;
                Log.Error(message);
                return BadRequest(new { message });
            }
        }
    }
}