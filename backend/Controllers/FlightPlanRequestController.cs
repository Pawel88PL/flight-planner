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
        public async Task<IActionResult> AddFlightPlanRequestAsync(FlightPlanRequest flightPlanRequest)
        {
            try
            {
                await _flightPlanRequestService.AddFlightPlanRequestAsync(flightPlanRequest);
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Wystąpił błąd podczas dodawania nowego zapytania o plan lotu");
                return BadRequest(ex.Message);
            }
        }
    }
}