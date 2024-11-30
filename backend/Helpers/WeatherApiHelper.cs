using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using backend.Interfaces;
using Serilog;

namespace backend.Helpers
{
    public class WeatherApiHelper : IWeatherApiHelper
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public WeatherApiHelper(HttpClient httpClient, IConfiguration configuration)
        {
            _configuration = configuration;

            // Konfiguracja HttpClient
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            // Dodaj nagłówek autoryzacji
            AddApiKeyHeader();
        }

        public async Task<T> GetAsync<T>(string metarOrTaf, string departureICAO, string arrivalICAO)
        {
            try
            {

                var url = $"https://api.checkwx.com/{metarOrTaf}/{departureICAO},{arrivalICAO}";

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Log.Error("Request failed with status code {StatusCode}: {ErrorContent}",
                        response.StatusCode, errorContent);
                    throw new HttpRequestException(
                        $"Request failed with status code {response.StatusCode}");
                }

                var responseData = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<T>(responseData)
                    ?? throw new InvalidOperationException("Failed to deserialize response data.");
            }
            catch (HttpRequestException httpRequestException)
            {
                Log.Error("An error occurred while sending HTTP GET request. {Message}", httpRequestException.Message);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error("An unexpected error occurred. {Message}", ex.Message);
                throw;
            }
        }

        private void AddApiKeyHeader()
        {
            var apiKey = _configuration["WeatherApi:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("API key is missing in configuration.");
            }

            // Użyj nagłówka X-API-Key zamiast Authorization
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);
        }
    }
}