using backend.Data;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Polly;
using Serilog;

namespace backend.Services
{
    public class AIService : IAIService
    {
        private readonly IConfiguration _configuration;
        private readonly IOpenAIHelper _openAIHelper;

        public AIService(IConfiguration configuration, IOpenAIHelper openAIHelper)
        {
            _configuration = configuration;
            _openAIHelper = openAIHelper;
        }


        public async Task<string> CreateJustification(WeatherResponse weatherResponse)
        {
            var departureMetar = weatherResponse.DepartureMETAR;
            var arrivalMetar = weatherResponse.ArrivalMETAR;
            var departureTaf = weatherResponse.DepartureTAF;
            var arrivalTaf = weatherResponse.ArrivalTAF;

            var justificationRequest = new AIRequest
            {
                model = _configuration["OpenAI:Model"],
                messages = new List<AIMessages>
                {
                    new AIMessages
                    {
                        role = "system",
                        content = "Jesteś ekspertem w dziedzinie lotnictwa. Analizujesz dane pogodowe lotniska startu i docelowego. "
                                + "Na podstawie tych danych trzeba podjąć decyzję czy lot VFR jest bezpieczny stosując się do przepisów VFR."
                    },
                    new AIMessages
                    {
                        role = "user",
                        content = $"Departure METAR: {departureMetar} Arrival METAR: {arrivalMetar} Departure TAF: {departureTaf} Arrival TAF: {arrivalTaf}"
                    }
                }
            };


            var responseData = await _openAIHelper.SentRequestToOpenAI(justificationRequest);


            var response = JsonConvert.DeserializeObject<string>(responseData)
                ?? throw new JsonSerializationException("Invalid JSON response from AI");

            Log.Information("Uzasadnienie wygenerowane przez AI: ", response);
            return response;
        }
    }
}