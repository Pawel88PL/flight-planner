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
        private readonly AIRepository _aiRepository;
        private readonly IConfiguration _configuration;
        private readonly IFlightPlanService _flightPlanService;
        private readonly IOpenAIHelper _openAIHelper;

        public AIService(
            AIRepository aiRepository,
            IConfiguration configuration,
            IFlightPlanService flightPlanService,
            IOpenAIHelper openAIHelper)
        {
            _aiRepository = aiRepository;
            _configuration = configuration;
            _flightPlanService = flightPlanService;
            _openAIHelper = openAIHelper;
        }

        private async Task<int> AddToDatabase(int flightPlanId, string response)
        {
            var responseId = await _aiRepository.AddToDatabaseAsync(response, flightPlanId);

            return responseId;
        }

        private async Task<string> CreateJustification(Dictionary<string, object> flightPlanData)
        {
            var flightPlan = JsonConvert.SerializeObject(flightPlanData);

            Log.Information("Dane lotu do analizy: {FlightPlan}", flightPlan);

            var justificationRequest = new AIRequest
            {
                model = _configuration["OpenAI:Model"],
                messages = new List<AIMessages>
                {
                    new AIMessages
                    {
                        role = "system",
                        content = "Jesteś ekspertem w dziedzinie lotnictwa i specjalizujesz się w analizie danych pogodowych. "
                                + "Twoim zadaniem jest ocenić bezpieczeństwo lotu VFR (Visual Flight Rules) na podstawie daty i godziny lotu, danych METAR i TAF. "
                                + "Przeanalizuj warunki pogodowe, takie jak widoczność, podstawa chmur, wiatr oraz inne kluczowe parametry. "
                                + "Na podstawie podanych danych oceń, czy lot VFR może zostać wykonany w bezpieczny sposób, stosując się do przepisów VFR. "
                                + "Podaj jednoznaczną decyzję (czy lot jest możliwy) oraz uzasadnij swoją decyzję. Odpowiedź ma być w języku polskim."
                    },
                    new AIMessages
                    {
                        role = "user",
                        content = $"Oto dane planowanego lotu do analizy:\n"
                                + $"{flightPlan}"
                                + "Czy lot VFR jest możliwy? Proszę podać szczegółowe uzasadnienie w języku polskim."
                    }
                }
            };

            var responseData = await _openAIHelper.SentRequestToOpenAI(justificationRequest);

            Log.Information("Uzasadnienie wygenerowane przez AI (PL): {Response}", responseData);
            return responseData;
        }

        private async Task GenerateAIResponse(int flightPlanId)
        {
            var request = await PrepareAIRequest(flightPlanId);
            var response = await CreateJustification(request);
            await AddToDatabase(flightPlanId, response);
        }

        public async Task<AIResponseDto> GetAIResponseByFlightPlanId(int flightPlanId)
        {
            var aiResponse = await _aiRepository.GetAIResponseByFlightPlanId(flightPlanId);

            if (aiResponse == null)
            {
                await GenerateAIResponse(flightPlanId);
                aiResponse = await _aiRepository.GetAIResponseByFlightPlanId(flightPlanId);
            }

            var aiResponseDto = new AIResponseDto
            {
                Response = aiResponse!.Response,
            };

            return aiResponseDto;
        }

        private async Task<Dictionary<string, object>> PrepareAIRequest(int flightPlanId)
        {
            var flightPlan = await _flightPlanService.GetFlightPlan(flightPlanId);

            return new Dictionary<string, object>
            {
                { "Dzień lotu", GetValueOrDefault(flightPlan.FlightDay) },
                { "Czas trwania lotu", GetValueOrDefault(flightPlan.FlightDuration) },
                { "Godzina wylotu", GetValueOrDefault(flightPlan.DepartureTime) },

                { "Kod lotniska startu", GetValueOrDefault(flightPlan.DepartureAirport.ICAO) },
                { "Miasto lotniska startu", GetValueOrDefault(flightPlan.DepartureAirport.City) },
                { "Kraj lotniska startu", GetValueOrDefault(flightPlan.DepartureAirport.Country) },
                { "METAR lotniska startu", GetValueOrDefault(flightPlan.DepartureAirport.METAR) },
                { "TAF lotniska startu", GetValueOrDefault(flightPlan.DepartureAirport.TAF) },
                { "Nazwa lotniska startu", GetValueOrDefault(flightPlan.DepartureAirport.Name) },
                
                { "Kod lotniska lądowania", GetValueOrDefault(flightPlan.ArrivalAirport.ICAO) },
                { "Miasto lotniska lądowania", GetValueOrDefault(flightPlan.ArrivalAirport.City) },
                { "Kraj lotniska lądowania", GetValueOrDefault(flightPlan.ArrivalAirport.Country) },
                { "METAR lotniska lądowania", GetValueOrDefault(flightPlan.ArrivalAirport.METAR) },
                { "TAF lotniska lądowania", GetValueOrDefault(flightPlan.ArrivalAirport.TAF) },
                { "Nazwa lotniska lądowania", GetValueOrDefault(flightPlan.ArrivalAirport.Name) }
            };
        }

        private object GetValueOrDefault(object? value, string defaultValue = "Nie podano")
        {
            return value ?? defaultValue;
        }
    }
}