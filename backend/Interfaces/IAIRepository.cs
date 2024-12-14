using backend.Models;

namespace backend.Interfaces
{
    public interface IAIRepository
    {
        Task<int> AddToDatabaseAsync(string response, int flightPlanId);
        Task<AIResponse?> GetAIResponseByFlightPlanId(int flightPlanId);
    }
}