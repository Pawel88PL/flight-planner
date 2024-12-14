using backend.Data;
using backend.Interfaces;
using backend.Models;
using Serilog;

namespace backend.Services
{
    public class FlightPlanService : IFlightPlanService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAirportService _airportService;
        private readonly IAirportRepository _airportRepository;
        private readonly IFlightPlanRepository _flightPlanRepository;
        private readonly IWeatherService _weatherService;

        public FlightPlanService(
            ApplicationDbContext context,
            IAirportService airportService,
            IAirportRepository airportRepository,
            IFlightPlanRepository flightPlanRepository,
            IWeatherService weatherService)
        {
            _airportService = airportService;
            _airportRepository = airportRepository;
            _context = context;
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

            await Task.WhenAll(airportsTask, weatherTask);

            var airports = await airportsTask;
            var weather = await weatherTask;

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var airportsIds = await _airportRepository.AddAirportsToDatabase(airports, weather);
                var flightPlanId = await _flightPlanRepository.AddFlightPlanAsync(request, airportsIds);

                await transaction.CommitAsync();

                return flightPlanId;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while creating flight plan.");
                await transaction.RollbackAsync();
                throw;
            }
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