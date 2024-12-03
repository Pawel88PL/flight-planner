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
                        content = "Jesteś ekspertem w dziedzinie lotnictwa i specjalizujesz się w analizie danych pogodowych. "
                                + "Twoim zadaniem jest ocenić bezpieczeństwo lotu VFR (Visual Flight Rules) na podstawie danych METAR i TAF. "
                                + "Przeanalizuj warunki pogodowe, takie jak widoczność, podstawa chmur, wiatr oraz inne kluczowe parametry. "
                                + "Na podstawie podanych danych oceń, czy lot VFR może zostać wykonany w bezpieczny sposób, stosując się do przepisów VFR. "
                                + "Podaj jednoznaczną decyzję (czy lot jest możliwy) oraz uzasadnij swoją decyzję. Odpowiedź ma być w języku polskim."
                    },
                    new AIMessages
                    {
                        role = "user",
                        content = $"Oto dane pogodowe do analizy:\n"
                                + $"METAR lotniska wylotu: {departureMetar}\n"
                                + $"METAR lotniska docelowego: {arrivalMetar}\n"
                                + $"TAF lotniska wylotu: {departureTaf}\n"
                                + $"TAF lotniska docelowego: {arrivalTaf}\n\n"
                                + "Czy lot VFR jest możliwy? Proszę podać szczegółowe uzasadnienie w języku polskim."
                    }
                }
            };

            var responseData = await _openAIHelper.SentRequestToOpenAI(justificationRequest);

            Log.Information("Uzasadnienie wygenerowane przez AI (PL): {Response}", responseData);
            return responseData;
        }
    }
}