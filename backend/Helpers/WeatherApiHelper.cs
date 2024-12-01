using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using backend.Interfaces;
using Serilog;

namespace backend.Helpers
{
    public class WeatherApiHelper : IWeatherApiHelper
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public WeatherApiHelper(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiKey = configuration["WeatherApi:ApiKey"] ?? throw new InvalidOperationException("API key is missing.");

            // Konfiguracja HttpClient
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
        }

        public async Task<T> GetAsync<T>(string request, string departureICAO, string arrivalICAO)
        {
            var url = $"https://api.checkwx.com/{request}/{departureICAO},{arrivalICAO}";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Log.Error("Request failed with status code {StatusCode}: {ErrorContent}", response.StatusCode, errorContent);

                    if (response.StatusCode == (HttpStatusCode)429)
                    {
                        throw new HttpRequestException("API rate limit exceeded.");
                    }

                    throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
                }

                using var responseStream = await response.Content.ReadAsStreamAsync();

                return await JsonSerializer.DeserializeAsync<T>(responseStream)
                    ?? throw new InvalidOperationException("Failed to deserialize response data.");
            }
            catch (Exception ex)
            {
                Log.Error("An unexpected error occurred. {Message}", ex.Message);
                throw;
            }
        }
    }
}