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

            var weatherTask = _weatherService.GetWeatherDataForDepartureAndArrival(request.DepartureICAO, request.ArrivalICAO);
            var airportsTask = _airportService.GetDepartureAndArrivalAirports(request.DepartureICAO, request.ArrivalICAO);

            await Task.WhenAll(weatherTask, airportsTask);

            var weather = await weatherTask;
            var airports = await airportsTask;

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
            };

            return flightPlanDto;
        }

        public async Task<List<FlightPlanDto>> GetFlightPlansForUser(string userId)
        {
            var flightPlans = await _flightPlanRepository.GetFlightPlansForUser(userId);

            var flightPlansDto = flightPlans.Select(f => new FlightPlanDto
            {
                Id = f.Id,
                DepartureTime = f.DepartureTime,
                FlightDay = f.FlightDay,
                FlightDuration = f.FlightDuration,
                AircraftId = f.AircraftId,
                DepartureAirport = f.DepartureAirport,
                ArrivalAirport = f.ArrivalAirport,
            }).ToList();

            return flightPlansDto;
        }
    }
}