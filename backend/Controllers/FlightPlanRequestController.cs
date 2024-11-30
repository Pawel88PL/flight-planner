using backend.Interfaces;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/flight-plan-request")]
    public class FlightPlanRequestController : Controller
    {
        private readonly IFlightPlanRequestService _flightPlanRequestService;

        public FlightPlanRequestController(IFlightPlanRequestService flightPlanRequestService)
        {
            _flightPlanRequestService = flightPlanRequestService;
        }


        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateFlightPlanRequestAsync(FlightPlanRequest flightPlanRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _flightPlanRequestService.CreateFlightPlanRequest(flightPlanRequest);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var message = "Wystąpił błąd podczas dodawania nowego zapytania o plan lotu. " + ex.Message;
                Log.Error(message);
                return BadRequest(new { message });
            }
        }
    }
}