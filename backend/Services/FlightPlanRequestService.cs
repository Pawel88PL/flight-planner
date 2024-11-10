using backend.Data;
using backend.Interfaces;
using backend.Models;
using DocumentFormat.OpenXml.ExtendedProperties;

namespace backend.Services
{
    public class FlightPlanRequestService : IFlightPlanRequestService
    {
        private readonly ApplicationDbContext _context;

        public FlightPlanRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddFlightPlanRequestAsync(FlightPlanRequest flightPlanRequest)
        {
            var newFlightPlanRequest = new FlightPlanRequest
            {
                DepartureICAO = flightPlanRequest.DepartureICAO,
                ArrivalICAO = flightPlanRequest.ArrivalICAO,
                DepartureTime = flightPlanRequest.DepartureTime,
                FlightDay = flightPlanRequest.FlightDay,
                FlightDuration = flightPlanRequest.FlightDuration,
                AircraftId = flightPlanRequest.AircraftId,
                FetchWeatherData = flightPlanRequest.FetchWeatherData
            };

            _context.FlightPlanRequests.Add(newFlightPlanRequest);
            await _context.SaveChangesAsync();
        }
    }
}