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

        private async Task<FlightPlanRequestDto> AddFlightPlanRequestAsync(FlightPlanRequest flightPlanRequest)
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

            return MapFlightPlanRequestToDto(newFlightPlanRequest);

        }

        public async Task<string> CreateFlightPlanRequest(FlightPlanRequest flightPlanRequest)
        {
            if (string.IsNullOrEmpty(flightPlanRequest.DepartureICAO)
                || !string.IsNullOrEmpty(flightPlanRequest.ArrivalICAO))
            {
                throw new Exception("Departure and Arrival ICAO codes are required.");
            }

            var newFlightPlanRequest = await AddFlightPlanRequestAsync(flightPlanRequest);

            if (newFlightPlanRequest.DepartureICAO == null || newFlightPlanRequest.ArrivalICAO == null)
            {
                throw new Exception("Departure and Arrival ICAO codes are required.");
            }

            var weatherData = await GetWeatherData(newFlightPlanRequest.DepartureICAO, newFlightPlanRequest.ArrivalICAO);

            return weatherData;
        }

        private async Task<string> GetWeatherData(string departureICAO, string arrivalICAO)
        {
            var weather = await _weatherApiHelper.GetAsync<object>(departureICAO, arrivalICAO);

            return JsonSerializer.Serialize(new { weather });
        }

        public FlightPlanRequestDto MapFlightPlanRequestToDto(FlightPlanRequest flightPlanRequest)
        {
            return new FlightPlanRequestDto
            {
                DepartureICAO = flightPlanRequest.DepartureICAO,
                ArrivalICAO = flightPlanRequest.ArrivalICAO,
                DepartureTime = flightPlanRequest.DepartureTime,
                FlightDay = flightPlanRequest.FlightDay,
                FlightDuration = flightPlanRequest.FlightDuration,
                AircraftId = flightPlanRequest.AircraftId,
                FetchWeatherData = flightPlanRequest.FetchWeatherData
            };
        }
    }
}