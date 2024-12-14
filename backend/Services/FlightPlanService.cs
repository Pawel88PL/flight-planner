using backend.Data;
using backend.Interfaces;
using backend.Models;
using Serilog;

namespace backend.Services
{
    public class FlightPlanService : IFlightPlanService
    {
        private readonly IAirportService _airportService;
        private readonly IAirportRepository _airportRepository;
        private readonly IFlightPlanRepository _flightPlanRepository;
        private readonly IWeatherRepository _weatherRepository;
        private readonly IWeatherService _weatherService;

        public FlightPlanService(
            IAirportService airportService,
            IAirportRepository airportRepository,
            IFlightPlanRepository flightPlanRepository,
            IWeatherRepository weatherRepository,
            IWeatherService weatherService)
        {
            _airportService = airportService;
            _airportRepository = airportRepository;
            _flightPlanRepository = flightPlanRepository;
            _weatherRepository = weatherRepository;
            _weatherService = weatherService;
        }

        public async Task<int> CreateFlightPlan(FlightPlanRequest request)
        {
            if (string.IsNullOrEmpty(request.DepartureICAO) || string.IsNullOrEmpty(request.ArrivalICAO))
            {
                throw new Exception("Departure and Arrival ICAO codes are required.");
            }

            var airports = await _airportService.GetDepartureAndArrivalAirports(request.DepartureICAO, request.ArrivalICAO);
            var weather = await _weatherService.GetWeatherDataForDepartureAndArrival(request.DepartureICAO, request.ArrivalICAO);

            var airportsIds = await _airportRepository.AddAirportsToDatabase(airports);
            var flightPlanId = await _flightPlanRepository.AddFlightPlanAsync(request, airportsIds);

            await _weatherRepository.AddArrivalAndDepartureWeather(weather, airportsIds);

            return flightPlanId;
        }

        public async Task<FlightPlanDto> GetFlightPlan(int id)
        {
            var flightPlan = await _flightPlanRepository.GetFlightPlan(id);

            var flightPlanDto = new FlightPlanDto
            {
                Id = flightPlan.Id,
                DepartureTime = flightPlan.DepartureTime,
                FlightDay = flightPlan.FlightDay,
                FlightDuration = flightPlan.FlightDuration,
                AircraftId = flightPlan.AircraftId,
                DepartureAirport = flightPlan.DepartureAirport,
                ArrivalAirport = flightPlan.ArrivalAirport,
                AIJustification = flightPlan.AIResponse ?? new AIResponse(),
            };

            return flightPlanDto;
        }
    }
}