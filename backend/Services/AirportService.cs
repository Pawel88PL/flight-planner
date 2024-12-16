using backend.Data;
using backend.Interfaces;
using backend.Models;
using Newtonsoft.Json;

namespace backend.Services
{
    public class AirportService : IAirportService
    {
        private readonly IWeatherApiHelper _weatherApiHelper;

        public AirportService(IWeatherApiHelper weatherApiHelper)
        {
            _weatherApiHelper = weatherApiHelper;
        }

        public async Task<List<AirportData>> GetDepartureAndArrivalAirports(string departureICAO, string arrivalICAO)
        {
            // Pobranie danych lotnisk z API
            var airportsData = await _weatherApiHelper.GetAsync<object>("airport", departureICAO, arrivalICAO);

            var airports = airportsData.ToString();

            if (string.IsNullOrEmpty(airports))
            {
                throw new InvalidOperationException("Nie udało się pobrać danych lotnisk.");
            }

            // Parsowanie i wyodrębnianie danych lotnisk
            var airportsObject = JsonConvert.DeserializeObject<List<AirportsResponse>>(airports);
            if (airportsObject == null || airportsObject.Count == 0)
            {
                throw new InvalidOperationException("Nie udało się sparsować odpowiedzi lotnisk.");
            }

            // Znalezienie danych lotnisk dla lotniska odlotu i przylotu
            var departureAirport = airportsObject.FirstOrDefault(airport => airport.ICAO == departureICAO);
            var arrivalAirport = airportsObject.FirstOrDefault(airport => airport.ICAO == arrivalICAO);

            return new List<AirportData>
            {
                new AirportData
                {
                    ICAO = departureAirport?.ICAO ?? $"Nie znaleziono danych dla lotniska {departureICAO}.",
                    Name = departureAirport?.Name ?? $"Nie znaleziono danych dla lotniska {departureICAO}.",
                },
                new AirportData
                {
                    ICAO = arrivalAirport?.ICAO ?? $"Nie znaleziono danych dla lotniska {arrivalICAO}.",
                    Name = arrivalAirport?.Name ?? $"Nie znaleziono danych dla lotniska {arrivalICAO}."
                }
            };
        }
    }
}