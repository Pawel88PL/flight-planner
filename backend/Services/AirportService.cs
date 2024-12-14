using backend.Data;
using backend.Interfaces;
using backend.Models;
using Newtonsoft.Json;

namespace backend.Services
{
    public class AirportService : IAirportService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWeatherApiHelper _weatherApiHelper;

        public AirportService(ApplicationDbContext context, IWeatherApiHelper weatherApiHelper)
        {
            _context = context;
            _weatherApiHelper = weatherApiHelper;
        }

        public async Task GetDepartureAndArrivalAirports(string departureICAO, string arrivalICAO)
        {
            // Pobranie danych lotnisk z API
            var airportsData = await _weatherApiHelper.GetAsync<object>("station", departureICAO, arrivalICAO);

            var airports = airportsData.ToString();

            if (string.IsNullOrEmpty(airports))
            {
                throw new InvalidOperationException("Nie udało się pobrać danych lotnisk.");
            }

            // Parsowanie i wyodrębnianie danych lotnisk
            var airportsObject = JsonConvert.DeserializeObject<AirportsResponse>(airports);
            if (airportsObject == null || airportsObject.Data == null)
            {
                throw new InvalidOperationException("Nie udało się sparsować odpowiedzi lotnisk.");
            }

            // Znalezienie danych lotnisk dla lotniska odlotu i przylotu
            var departureAirport = airportsObject.Data.FirstOrDefault(airport => airport.ICAO == departureICAO);
            var arrivalAirport = airportsObject.Data.FirstOrDefault(airport => airport.ICAO == arrivalICAO);

            var airportList = new List<AirportData>
            {
                new AirportData
                {
                    ICAO = departureAirport?.ICAO ?? $"Nie znaleziono danych dla lotniska {departureICAO}.",
                    City = departureAirport?.City ?? $"Nie znaleziono danych dla lotniska {departureICAO}.",
                    Country = departureAirport?.Country ?? new Country { Code = "XX", Name = "Nieznany" },
                    Location = departureAirport?.Location ?? $"Nie znaleziono danych dla lotniska {departureICAO}.",
                    Name = departureAirport?.Name ?? $"Nie znaleziono danych dla lotniska {departureICAO}."
                },
                new AirportData
                {
                    ICAO = arrivalAirport?.ICAO ?? $"Nie znaleziono danych dla lotniska {arrivalICAO}.",
                    City = arrivalAirport?.City ?? $"Nie znaleziono danych dla lotniska {arrivalICAO}.",
                    Country = arrivalAirport?.Country ?? new Country { Code = "XX", Name = "Nieznany" },
                    Location = arrivalAirport?.Location ?? $"Nie znaleziono danych dla lotniska {arrivalICAO}.",
                    Name = arrivalAirport?.Name ?? $"Nie znaleziono danych dla lotniska {arrivalICAO}."
                }
            };

            await AddAirportsToDatabase(airportList);
        }

        private async Task AddAirportsToDatabase(List<AirportData> airports)
        {
            var departureAirport = new DepartureAirport
            {
                ICAO = airports[0].ICAO,
                Name = airports[0].Name,
                City = airports[0].City,
                Country = airports[0].Country.Name,
            };

            var arrivalAirport = new ArrivalAirport
            {
                ICAO = airports[1].ICAO,
                Name = airports[1].Name,
                City = airports[1].City,
                Country = airports[1].Country.Name,
            };

            _context.DepartureAirports.Add(departureAirport);
            _context.ArrivalAirports.Add(arrivalAirport);

            await _context.SaveChangesAsync();
        }
    }
}