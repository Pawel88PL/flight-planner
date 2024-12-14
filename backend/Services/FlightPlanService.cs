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
        private readonly IWeatherService _weatherService;

        public FlightPlanService(
            IAirportService airportService,
            IAirportRepository airportRepository,
            IFlightPlanRepository flightPlanRepository,
            IWeatherService weatherService)
        {
            _airportService = airportService;
            _airportRepository = airportRepository;
            _flightPlanRepository = flightPlanRepository;
            _weatherService = weatherService;
        }

        public async Task<int> CreateFlightPlan(FlightPlanRequest request)
        {
            if (string.IsNullOrEmpty(request.DepartureICAO) || string.IsNullOrEmpty(request.ArrivalICAO))
            {
                throw new Exception("Departure and Arrival ICAO codes are required.");
            }

            var airportsTask = _airportService.GetDepartureAndArrivalAirports(request.DepartureICAO, request.ArrivalICAO);
            var weatherTask = _weatherService.GetWeatherDataForDepartureAndArrival(request.DepartureICAO, request.ArrivalICAO);

            try
            {
                await Task.WhenAll(airportsTask, weatherTask);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to fetch data from external services.");
                throw new Exception("Failed to fetch data from external services.", ex);
            }

            await _airportRepository.AddAirportsToDatabase(airportsTask.Result);

            var flightPlanId = await _flightPlanRepository.AddFlightPlanAsync(request);
            
            return flightPlanId;
        }

        public async Task<FlightPlanDto> GetFlightPlan(int id)
        {
            var flightPlan = await _flightPlanRepository.GetFlightPlan(id);

            return new FlightPlanDto
            {
                Id = flightPlan.Id,
                DepartureTime = flightPlan.DepartureTime,
                FlightDay = flightPlan.FlightDay,
                FlightDuration = flightPlan.FlightDuration,
                AircraftId = flightPlan.AircraftId,
                DepartureAirport = new DepartureAirport
                {
                    ICAO = flightPlan.DepartureAirport.ICAO,
                    City = flightPlan.DepartureAirport.City,
                    Country = flightPlan.DepartureAirport.Country,
                    Name = flightPlan.DepartureAirport.Name,
                    METAR = flightPlan.DepartureAirport.METAR,
                    TAF = flightPlan.DepartureAirport.TAF
                },
                ArrivalAirport = new ArrivalAirport
                {
                    ICAO = flightPlan.ArrivalAirport.ICAO,
                    City = flightPlan.ArrivalAirport.City,
                    Country = flightPlan.ArrivalAirport.Country,
                    Name = flightPlan.ArrivalAirport.Name,
                    METAR = flightPlan.ArrivalAirport.METAR,
                    TAF = flightPlan.ArrivalAirport.TAF
                }
            };
        }
    }
}