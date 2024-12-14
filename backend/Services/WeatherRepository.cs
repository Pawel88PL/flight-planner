using backend.Data;
using backend.Interfaces;
using backend.Models;

namespace backend.Services
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly ApplicationDbContext _context;

        public WeatherRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddArrivalAndDepartureWeather(WeatherResponse weatherResponse, List<int> airportsIds)
        {
            int departureAirportId = airportsIds[0];
            int arrivalAirportId = airportsIds[1];

            var departureAirport = _context.DepartureAirports
                .Where(dep => dep.Id == departureAirportId)
                .FirstOrDefault();

            var arrivalAirport = _context.ArrivalAirports
                .Where(arr => arr.Id == arrivalAirportId)
                .FirstOrDefault();
            
            if (departureAirport == null || arrivalAirport == null)
            {
                throw new Exception("Departure or Arrival airport not found.");
            }

            departureAirport.METAR = weatherResponse.DepartureMETAR;
            departureAirport.TAF = weatherResponse.DepartureTAF;

            arrivalAirport.METAR = weatherResponse.ArrivalMETAR;
            arrivalAirport.TAF = weatherResponse.ArrivalTAF;

            await _context.SaveChangesAsync();

        }
    }
}