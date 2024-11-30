using backend.Interfaces;
using backend.Models;
using Newtonsoft.Json;

namespace backend.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IWeatherApiHelper _weatherApiHelper;

        public WeatherService(IWeatherApiHelper weatherApiHelper)
        {
            _weatherApiHelper = weatherApiHelper;
        }

        private async Task<WeatherResponse> GetMetarForDepartureAndArrival(string departureICAO, string arrivalICAO)
        {
            // Pobranie danych METAR z API
            var weatherData = await _weatherApiHelper.GetAsync<object>("metar", departureICAO, arrivalICAO);

            var metarData = weatherData.ToString();

            if (string.IsNullOrEmpty(metarData))
            {
                throw new InvalidOperationException("Nie udało się pobrać danych METAR.");
            }

            // Parsowanie i wyodrębnianie METAR dla poszczególnych lotnisk
            var weatherObject = JsonConvert.DeserializeObject<MetarResponse>(metarData);
            if (weatherObject == null || weatherObject.Data == null)
            {
                throw new InvalidOperationException("Nie udało się sparsować odpowiedzi METAR.");
            }

            // Znalezienie danych METAR dla lotniska odlotu
            var departureMETAR = weatherObject.Data.FirstOrDefault(metar => metar.Contains(departureICAO));
            var arrivalMETAR = weatherObject.Data.FirstOrDefault(metar => metar.Contains(arrivalICAO));

            // Zwrot wyniku
            return new WeatherResponse
            {
                DepartureMETAR = departureMETAR ?? $"Nie znaleziono danych dla lotniska {departureICAO}.",
                ArrivalMETAR = arrivalMETAR ?? $"Nie znaleziono danych dla lotniska {arrivalICAO}."
            };
        }


        private async Task<WeatherResponse> GetTafForDepartureAndArrival(string departureICAO, string arrivalICAO)
        {
            // Pobranie danych TAF z API
            var weatherData = await _weatherApiHelper.GetAsync<object>("taf", departureICAO, arrivalICAO);

            var tafData = weatherData.ToString();

            if (string.IsNullOrEmpty(tafData))
            {
                throw new InvalidOperationException("Nie udało się pobrać danych TAF.");
            }

            var tafObject = JsonConvert.DeserializeObject<TafResponse>(tafData);
            if (tafObject == null || tafObject.Data == null)
            {
                throw new InvalidOperationException("Nie udało się sparsować odpowiedzi TAF.");
            }

            // Znalezienie danych TAF dla lotniska odlotu
            var departureTAF = tafObject.Data.FirstOrDefault(taf => taf.Contains(departureICAO));
            var arrivalTAF = tafObject.Data.FirstOrDefault(taf => taf.Contains(arrivalICAO));

            // Zwrot wyniku
            return new WeatherResponse
            {
                DepartureTAF = departureTAF ?? $"Nie znaleziono danych dla lotniska {departureICAO}.",
                ArrivalTAF = arrivalTAF ?? $"Nie znaleziono danych dla lotniska {arrivalICAO}."
            };
        }

        public async Task<WeatherResponse> GetWeatherDataForDepartureAndArrival(string departureICAO, string arrivalICAO)
        {
            var metar = await GetMetarForDepartureAndArrival(departureICAO, arrivalICAO);
            var taf = await GetTafForDepartureAndArrival(departureICAO, arrivalICAO);

            return new WeatherResponse
            {
                DepartureMETAR = metar.DepartureMETAR,
                ArrivalMETAR = metar.ArrivalMETAR,
                DepartureTAF = taf.DepartureTAF,
                ArrivalTAF = taf.ArrivalTAF
            };
        }
    }
}