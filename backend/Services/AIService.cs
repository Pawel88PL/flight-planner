using backend.Interfaces;
using backend.Models;
using Newtonsoft.Json;

namespace backend.Services
{
    public class AIService : IAIService
    {
        private readonly IAIRepository _aiRepository;
        private readonly IAircraftService _aircraftService;
        private readonly IConfiguration _configuration;
        private readonly IFlightPlanService _flightPlanService;
        private readonly IOpenAIHelper _openAIHelper;

        public AIService(
            IAIRepository aiRepository,
            IAircraftService aircraftService,
            IConfiguration configuration,
            IFlightPlanService flightPlanService,
            IOpenAIHelper openAIHelper)
        {
            _aiRepository = aiRepository;
            _aircraftService = aircraftService;
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

            var systemContent =
            @"Jesteś doświadczonym doradcą lotniczym i ekspertem w analizie warunków pogodowych dla lotów VFR (Visual Flight Rules). Twoim zadaniem jest ocena bezpieczeństwa i możliwości wykonania planowanego lotu w oparciu o:

            1. Aktualne i prognozowane warunki meteorologiczne (METAR/TAF) dla lotnisk startu, docelowego.
            2. Minimalne wymogi VFR w zakresie widoczności, podstawy chmur oraz warunków atmosferycznych.
            3. Ograniczenia statku powietrznego, w tym maksymalną dopuszczalną boczną składową wiatru (crosswind component), zasięg statku powietrznego i prędkość przelotową.
            4. Planowane parametry lotu (czas, data, trasa).
            5. Możliwe oblodzenie statku powietrznego w trakcie lotu.
            6. Inne czynniki, które mogą mieć wpływ na bezpieczeństwo lotu.

            Wynik swojej analizy przedstaw w języku polskim w następującej formie:
            - Najpierw podaj jednoznaczną decyzję, czy lot VFR jest możliwy, uwzględniając wszystkie podane dane.
            - Następnie uzasadnij tę decyzję, odnosząc się do konkretnych parametrów pogodowych (widzialność, chmury, wiatr), obowiązujących przepisów VFR oraz ograniczeń samolotu, w tym dopuszczalnego crosswind component, zasięgu i prędkości przelotowej.
            - Na koniec wymień potencjalne zagrożenia, mogące pojawić się w trakcie lotu i wydaj zalecenia dla pilota.

            Twoja odpowiedź powinna być merytoryczna, spójna i uwzględniać kluczowe aspekty bezpieczeństwa.
            
            Wyróżnij ją od reszty tekstu, np. poprzez zastosowanie dobrze widocznego nagłówka lub formatowania.";

            var userContent =
            @$"Oto dane planowanego lotu do analizy, łącznie z danymi wybranego samolotu (format JSON):
            {flightPlan}

            Czy lot VFR jest możliwy i bezpieczny? Proszę podać szczegółowe uzasadnienie w języku polskim, odnosząc się do podanych informacji o pogodzie oraz ograniczeniach samolotu.";

            var justificationRequest = new AIRequest
            {
                model = _configuration["OpenAI:Model"],
                temperature = 0.3m,
                max_tokens = 3000,
                messages = new List<AIMessages>
                {
                    new AIMessages
                    {
                        role = "system",
                        content = systemContent

                    },
                    new AIMessages
                    {
                        role = "user",
                        content = userContent
                    }
                }
            };

            var responseData = await _openAIHelper.SentRequestToOpenAI(justificationRequest);

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
            var aircraftModel = await _aircraftService.GetAircraftById(flightPlan.AircraftId);

            if (flightPlan == null || aircraftModel == null)
            {
                throw new Exception("Nie znaleziono planu lotu lub samolotu");
            }

            var aircraftData = new Dictionary<string, object>
            {
                { "Producent samolotu", GetValueOrDefault(aircraftModel.Manufacturer ) },
                { "Model samolotu", GetValueOrDefault(aircraftModel.Model) },
                { "Maksymalny boczny komponent wiatru w knots", GetValueOrDefault(aircraftModel.MaxCrosswind) },
                { "Zasięg w NM", GetValueOrDefault(aircraftModel.Range) },
                { "Prędkość przelotowa w knots", GetValueOrDefault(aircraftModel.CruiseSpeed) },
            };

            return new Dictionary<string, object>
            {
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
                { "Nazwa lotniska lądowania", GetValueOrDefault(flightPlan.ArrivalAirport.Name) },

                { "Dane samolotu", aircraftData }
            };
        }

        private static object GetValueOrDefault(object? value, string defaultValue = "Nie podano")
        {
            return value ?? defaultValue;
        }
    }
}