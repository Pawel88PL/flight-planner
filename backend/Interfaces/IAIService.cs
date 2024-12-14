using backend.Models;

namespace backend.Interfaces
{
    public interface IAIService
    {
        Task<AIResponseDto> GetAIResponseByFlightPlanId(int flightPlanId);
    }
}