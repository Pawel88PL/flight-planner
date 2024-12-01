using backend.Models;

namespace backend.Interfaces
{
    public interface IFlightPlanService
    {
        Task<int> CreateFlightPlan(FlightPlanRequest request);
        Task<FlightPlanResponseDto> GetFlightPlan(int id);
    }
}