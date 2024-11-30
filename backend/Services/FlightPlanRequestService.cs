using System.Text.Json;
using backend.Data;
using backend.Interfaces;
using backend.Models;

namespace backend.Services
{
    public class FlightPlanRequestService : IFlightPlanRequestService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWeatherService _weatherService;

        public FlightPlanRequestService(ApplicationDbContext context, IWeatherService weatherService)
        {
            _context = context;
            _weatherService = weatherService;
        }

        private async Task<FlightPlanResponse> AddFlightPlanRequestAsync(FlightPlanRequest flightPlanRequest, WeatherResponse weather)
        {
            var newFlightPlanRequest = new FlightPlanRequest
            {
                DepartureICAO = flightPlanRequest.DepartureICAO,
                ArrivalICAO = flightPlanRequest.ArrivalICAO,
                DepartureTime = flightPlanRequest.DepartureTime,
                FlightDay = flightPlanRequest.FlightDay,
                FlightDuration = flightPlanRequest.FlightDuration,
                AircraftId = flightPlanRequest.AircraftId,
                FetchWeatherData = flightPlanRequest.FetchWeatherData,
                DepartureMETAR = weather.DepartureMETAR,
                ArrivalMETAR = weather.ArrivalMETAR,
                DepartureTAF = weather.DepartureTAF,
                ArrivalTAF = weather.ArrivalTAF
            };

            _context.FlightPlanRequests.Add(newFlightPlanRequest);

            await _context.SaveChangesAsync();

            return MapFlightPlanRequestToFlightPlanResponse(newFlightPlanRequest);
        }

        public async Task<FlightPlanResponse> CreateFlightPlanRequest(FlightPlanRequest flightPlanRequest)
        {
            if (string.IsNullOrEmpty(flightPlanRequest.DepartureICAO)
                || string.IsNullOrEmpty(flightPlanRequest.ArrivalICAO))
            {
                throw new Exception("Departure and Arrival ICAO codes are required.");
            }

            var weatherData = await _weatherService.GetWeatherDataForDepartureAndArrival(flightPlanRequest.DepartureICAO, flightPlanRequest.ArrivalICAO);

            var newFlightPlanResponse = await AddFlightPlanRequestAsync(flightPlanRequest, weatherData);

            return newFlightPlanResponse;
        }

        private static FlightPlanResponse MapFlightPlanRequestToFlightPlanResponse(FlightPlanRequest flightPlanRequest)
        {
            return new FlightPlanResponse
            {
                ResponseId = flightPlanRequest.Id,
                DepartureICAO = flightPlanRequest.DepartureICAO,
                ArrivalICAO = flightPlanRequest.ArrivalICAO,
                DepartureTime = flightPlanRequest.DepartureTime,
                FlightDay = flightPlanRequest.FlightDay,
                FlightDuration = flightPlanRequest.FlightDuration,
                FetchWeatherData = flightPlanRequest.FetchWeatherData,
                DepartureMETAR = flightPlanRequest.DepartureMETAR,
                ArrivalMETAR = flightPlanRequest.ArrivalMETAR,
                DepartureTAF = flightPlanRequest.DepartureTAF,
                ArrivalTAF = flightPlanRequest.ArrivalTAF
            };
        }
    }
}