using System.Net.Http.Headers;
using System.Text;
using backend.Interfaces;
using backend.Models;
using Newtonsoft.Json;
using Polly;
using Serilog;

namespace backend.Helpers
{
    public class OpenAIHelper : IOpenAIHelper
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public OpenAIHelper(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<string> SentRequestToOpenAI(AIRequest request)
        {
            var apiKey = _configuration["OpenAI:ApiKey"];
            var url = _configuration["OpenAI:Url"];

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("OpenAI API key is missing");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()  // Dla przypadków timeoutu
                .WaitAndRetryAsync(10, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Log.Warning($"Retry {retryCount} encountered an error: {exception.Message}. Waiting {timeSpan} before next retry.");
                    });

            HttpResponseMessage response = null!;

            await retryPolicy.ExecuteAsync(async () =>
            {
                response = await _httpClient.PostAsync(url, content);
            });

            // Dodaj logowanie odpowiedzi
            var responseData = await response.Content.ReadAsStringAsync();
            Log.Information("OpenAI API response: {Response}", responseData);

            if (!response.IsSuccessStatusCode)
            {
                // Loguj szczegóły błędu
                Log.Error("OpenAI API request failed with status code {StatusCode} and message {Message}", response.StatusCode, responseData);
                throw new HttpRequestException($"OpenAI API request failed with status code {response.StatusCode}");
            }

            try
            {
                var result = JsonConvert.DeserializeObject<dynamic>(responseData)
                    ?? throw new Exception("Failed to deserialize OpenAI response");

                
                var contentResult = result.choices[0].message.content.ToString();
                return contentResult.Trim();
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while processing the OpenAI response: {Message}", ex.Message);
                throw;
            }
        }

    }
}