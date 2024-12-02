using backend.Data;
using backend.Interfaces;
using backend.Models;

namespace backend.Services
{
    public class FlightPlanService : IFlightPlanService
    {
        private readonly IAirportService _airportService;
        private readonly ApplicationDbContext _context;
        private readonly IWeatherService _weatherService;

        public FlightPlanService(ApplicationDbContext context, IAirportService airportService, IWeatherService weatherService)
        {
            _context = context;
            _airportService = airportService;
            _weatherService = weatherService;
        }

        private async Task<int> AddFlightPlanToDataBaseAsync(FlightPlanRequest flightPlanRequest, List<AirportData> airportDatas, WeatherResponse weather)
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
                ArrivalTAF = weather.ArrivalTAF,
                DepartureAirportName = airportDatas[0].Name,
                DepartureCity = airportDatas[0].City,
                DepartureCountry = airportDatas[0].Country.Name,
                ArrivalAirportName = airportDatas[1].Name,
                ArrivalCity = airportDatas[1].City,
                ArrivalCountry = airportDatas[1].Country.Name
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
            var airports = await _airportService.GetDepartureAndArrivalAirports(request.DepartureICAO, request.ArrivalICAO);

            var flightPlanResponseId = await AddFlightPlanToDataBaseAsync(request, airports, weatherData);

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

        private static FlightPlanResponseDto MapFlightPlanResponseToDto(FlightPlanResponse response)
        {
            return new FlightPlanResponseDto
            {
                DepartureICAO = response.DepartureICAO,
                ArrivalICAO = response.ArrivalICAO,
                DepartureTime = response.DepartureTime,
                FlightDay = response.FlightDay,
                FlightDuration = response.FlightDuration,
                DepartureMETAR = response.DepartureMETAR,
                ArrivalMETAR = response.ArrivalMETAR,
                DepartureTAF = response.DepartureTAF,
                ArrivalTAF = response.ArrivalTAF,
                AircraftId = response.AircraftId,
                DepartureAirportName = response.DepartureAirportName,
                DepartureCity = response.DepartureCity,
                DepartureCountry = response.DepartureCountry,
                ArrivalAirportName = response.ArrivalAirportName,
                ArrivalCity = response.ArrivalCity,
                ArrivalCountry = response.ArrivalCountry
            };
        }
    }
}