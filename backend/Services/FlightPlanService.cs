using backend.Data;
using backend.Interfaces;
using backend.Models;
using Serilog;

namespace backend.Services
{
    public class FlightPlanService : IFlightPlanService
    {
        private readonly IAirportService _airportService;
        private readonly IAIService _aiService;
        private readonly IFlightPlanRepository _flightPlanRepository;
        private readonly IWeatherService _weatherService;

        public FlightPlanService(
            IAirportService airportService,
            IAIService aiService,
            IFlightPlanRepository flightPlanRepository,
            IWeatherService weatherService)
        {
            _aiService = aiService;
            _airportService = airportService;
            _flightPlanRepository = flightPlanRepository;
            _weatherService = weatherService;
        }

        public async Task<int> CreateFlightPlan(FlightPlanRequest request)
        {
            if (string.IsNullOrEmpty(request.DepartureICAO) || string.IsNullOrEmpty(request.ArrivalICAO))
            {
                throw new Exception("Departure and Arrival ICAO codes are required.");
            }

            var flightPlanTask = _flightPlanRepository.AddFlightPlanAsync(request);
            var weatherTask = _weatherService.GetWeatherDataForDepartureAndArrival(request.DepartureICAO, request.ArrivalICAO);
            var airportsTask = _airportService.GetDepartureAndArrivalAirports(request.DepartureICAO, request.ArrivalICAO);

            try
            {
                await Task.WhenAll(flightPlanTask,weatherTask, airportsTask);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to fetch data from external services.");
                throw new Exception("Failed to fetch data from external services.", ex);
            }

            var flightPlanId = await flightPlanTask;
            
            return flightPlanId;
        }

        public async Task<FlightPlanDto> GetFlightPlan(int id)
        {
            var flightPlan = await _flightPlanRepository.GetFlightPlan(id);
            // var departureAirport = await _airportService.GetAirportByICAO(flightPlan.DepartureICAO);
            // var arrivalAirport = await _airportService.GetAirportByICAO(flightPlan.ArrivalICAO);
            // var departureWeather = await _weatherService.GetWeatherDataForICAO(flightPlan.DepartureICAO);
            // var arrivalWeather = await _weatherService.GetWeatherDataForICAO(flightPlan.ArrivalICAO);

            return new FlightPlanDto
            {
                Id = flightPlan.Id,
                DepartureICAO = flightPlan.DepartureICAO,
                ArrivalICAO = flightPlan.ArrivalICAO,
                DepartureTime = flightPlan.DepartureTime,
                FlightDay = flightPlan.FlightDay,
                FlightDuration = flightPlan.FlightDuration,
                AircraftId = flightPlan.AircraftId,
                CreatedAt = flightPlan.CreatedAt,
                // DepartureAirportName = departureAirport.Name,
                // DepartureCity = departureAirport.City,
                // DepartureCountry = departureAirport.Country,
                // DepartureMETAR = departureWeather.METAR,
                // DepartureTAF = departureWeather.TAF,
                // ArrivalMETAR = arrivalWeather.METAR,
                // ArrivalTAF = arrivalWeather.TAF,
            };
        }
    }
}