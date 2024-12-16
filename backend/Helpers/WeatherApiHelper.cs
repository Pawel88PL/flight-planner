using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using backend.Interfaces;
using Serilog;

namespace backend.Helpers
{
    public class WeatherApiHelper : IWeatherApiHelper
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public WeatherApiHelper(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<T> GetAsync<T>(string request, string departureICAO, string arrivalICAO)
        {
            var baseUrl = _configuration["AviationWeather:ApiUrl"]
                ?? throw new InvalidOperationException("API base URL is not configured.");

            var url = $"{baseUrl}/{request}?ids={departureICAO}%2C{arrivalICAO}&format=json&taf=true";

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