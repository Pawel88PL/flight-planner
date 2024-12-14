using backend.Data;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class AirportRepository : IAirportRepository
    {
        private readonly ApplicationDbContext _context;

        public AirportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<int>> AddAirportsToDatabase(List<AirportData> airports, WeatherResponse weather)
        {
            var departureAirport = new DepartureAirport
            {
                ICAO = airports[0].ICAO,
                Name = airports[0].Name,
                City = airports[0].City,
                Country = airports[0].Country.Name,
                METAR = weather.DepartureMETAR,
                TAF = weather.DepartureTAF
            };

            var arrivalAirport = new ArrivalAirport
            {
                ICAO = airports[1].ICAO,
                Name = airports[1].Name,
                City = airports[1].City,
                Country = airports[1].Country.Name,
                METAR = weather.ArrivalMETAR,
                TAF = weather.ArrivalTAF
            };

            _context.DepartureAirports.Add(departureAirport);
            _context.ArrivalAirports.Add(arrivalAirport);

            await _context.SaveChangesAsync();

            return new List<int> {departureAirport.Id, arrivalAirport.Id};
        }

        public async Task<ArrivalAirport?> GetArrivalAirportByICAO(string icao)
        {
            var airport = await _context.ArrivalAirports
                .Where(a => a.ICAO == icao)
                .FirstOrDefaultAsync();

            if (airport == null)
            {
                return null;
            }

            return new ArrivalAirport
            {
                ICAO = airport.ICAO,
                City = airport.City,
                Country = airport.Country,
                Name = airport.Name
            };
        }

        public async Task<DepartureAirport?> GetDepartureAirportByICAO(string icao)
        {
            var airport = await _context.DepartureAirports
                .Where(a => a.ICAO == icao)
                .FirstOrDefaultAsync();

            if (airport == null)
            {
                return null;
            }

            return new DepartureAirport
            {
                ICAO = airport.ICAO,
                City = airport.City,
                Country = airport.Country,
                Name = airport.Name
            };
        }
    }
}