using System.Text.Json;
using backend.Data;
using backend.Interfaces;
using backend.Models;
using DocumentFormat.OpenXml.ExtendedProperties;

namespace backend.Services
{
    public class FlightPlanRequestService : IFlightPlanRequestService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWeatherApiHelper _weatherApiHelper;

        public FlightPlanRequestService(ApplicationDbContext context, IWeatherApiHelper weatherApiHelper)
        {
            _context = context;
            _weatherApiHelper = weatherApiHelper;
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

        public async Task<string> GetWeatherData(string departureICAO, string arrivalICAO)
        {
            var weather = await _weatherApiHelper.GetAsync<object>(departureICAO, arrivalICAO);
    
            return JsonSerializer.Serialize(new { weather });
        }
    }
}