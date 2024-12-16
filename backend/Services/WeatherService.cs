using backend.Interfaces;
using backend.Models;
using Newtonsoft.Json;
using Serilog;

namespace backend.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IWeatherApiHelper _weatherApiHelper;

        public WeatherService(IWeatherApiHelper weatherApiHelper)
        {
            _weatherApiHelper = weatherApiHelper;
        }

        private async Task<WeatherResponse> GetWeatherForDepartureAndArrival(string departureICAO, string arrivalICAO)
        {
            // Pobranie danych METAR z API
            var weatherData = await _weatherApiHelper.GetAsync<object>("metar", departureICAO, arrivalICAO);

            var metarData = weatherData.ToString();

            if (string.IsNullOrEmpty(metarData))
            {
                throw new InvalidOperationException("Nie udało się pobrać danych METAR.");
            }

            // Parsowanie i wyodrębnianie METAR dla poszczególnych lotnisk
            var weatherObject = JsonConvert.DeserializeObject<List<WeatherData>>(metarData);
            if (weatherObject == null || weatherObject.Count == 0)
            {
                throw new InvalidOperationException("Nie udało się sparsować odpowiedzi METAR.");
            }

            // Znalezienie danych pogodowych dla lotniska odlotu i przylotu
            var departure = weatherObject.FirstOrDefault(metar => metar.ICAO == departureICAO);
            var arrival = weatherObject.FirstOrDefault(metar => metar.ICAO == arrivalICAO);

            
            if (departure == null || arrival == null)
            {
                throw new Exception("Nie znaleziono danych METAR dla podanych lotnisk.");
            }

            // Przykład użycia
            Log.Information($"Departure METAR: {departure.RawMetar}");
            Log.Information($"Arrival METAR: {arrival.RawMetar}");
            Log.Information($"Departure TAF: {departure.RawTaf}");
            Log.Information($"Arrival TAF: {arrival.RawTaf}");


            // Zwrot wyniku
            return new WeatherResponse
            {
                DepartureMETAR = departure.RawMetar!,
                DepartureTAF = departure.RawTaf!,
                ArrivalMETAR =  arrival.RawMetar!,
                ArrivalTAF = arrival.RawTaf!
            };
        }
        
        public async Task<WeatherResponse> GetWeatherDataForDepartureAndArrival(string departureICAO, string arrivalICAO)
        {
            var metar = await GetWeatherForDepartureAndArrival(departureICAO, arrivalICAO);

            return new WeatherResponse
            {
                DepartureMETAR = metar.DepartureMETAR,
                ArrivalMETAR = metar.ArrivalMETAR,
                DepartureTAF = metar.DepartureTAF,
                ArrivalTAF = metar.ArrivalTAF
            };
        }
    }
}