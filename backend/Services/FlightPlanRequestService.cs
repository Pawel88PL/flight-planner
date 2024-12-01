using backend.Data;
using backend.Interfaces;
using backend.Models;

namespace backend.Services
{
    public class FlightPlanService : IFlightPlanService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWeatherService _weatherService;

        public FlightPlanService(ApplicationDbContext context, IWeatherService weatherService)
        {
            _context = context;
            _weatherService = weatherService;
        }

        private async Task<int> AddFlightPlanToDataBaseAsync(FlightPlanRequest flightPlanRequest, WeatherResponse weather)
        {
            var newFlightPlan = new FlightPlanResponse
            {
                DepartureICAO = flightPlanRequest.DepartureICAO,
                ArrivalICAO = flightPlanRequest.ArrivalICAO,
                DepartureTime = flightPlanRequest.DepartureTime,
                FlightDay = flightPlanRequest.FlightDay,
                FlightDuration = flightPlanRequest.FlightDuration,
                AircraftId = flightPlanRequest.AircraftId,
                DepartureMETAR = weather.DepartureMETAR,
                ArrivalMETAR = weather.ArrivalMETAR,
                DepartureTAF = weather.DepartureTAF,
                ArrivalTAF = weather.ArrivalTAF
            };

            _context.FlightPlanResponses.Add(newFlightPlan);

            await _context.SaveChangesAsync();

            return newFlightPlan.Id;
        }

        public async Task<int> CreateFlightPlan(FlightPlanRequest request)
        {
            if (string.IsNullOrEmpty(request.DepartureICAO) || string.IsNullOrEmpty(request.ArrivalICAO))
            {
                throw new Exception("Departure and Arrival ICAO codes are required.");
            }

            var weatherData = await _weatherService.GetWeatherDataForDepartureAndArrival(request.DepartureICAO, request.ArrivalICAO);

            var flightPlanResponseId = await AddFlightPlanToDataBaseAsync(request, weatherData);

            return flightPlanResponseId;
        }

        public async Task<FlightPlanResponseDto> GetFlightPlan(int id)
        {
            var flightPlanResponse = await _context.FlightPlanResponses.FindAsync(id);

            if (flightPlanResponse == null)
            {
                throw new Exception("Flight plan request not found.");
            }

            return MapFlightPlanResponseToDto(flightPlanResponse);
        }

        private static FlightPlanResponseDto MapFlightPlanResponseToDto(FlightPlanResponse flightPlanRequest)
        {
            return new FlightPlanResponseDto
            {
                DepartureICAO = flightPlanRequest.DepartureICAO,
                ArrivalICAO = flightPlanRequest.ArrivalICAO,
                DepartureTime = flightPlanRequest.DepartureTime,
                FlightDay = flightPlanRequest.FlightDay,
                FlightDuration = flightPlanRequest.FlightDuration,
                DepartureMETAR = flightPlanRequest.DepartureMETAR,
                ArrivalMETAR = flightPlanRequest.ArrivalMETAR,
                DepartureTAF = flightPlanRequest.DepartureTAF,
                ArrivalTAF = flightPlanRequest.ArrivalTAF
            };
        }
    }
}