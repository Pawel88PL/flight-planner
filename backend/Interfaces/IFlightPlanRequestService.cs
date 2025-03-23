using backend.Models;

namespace backend.Interfaces
{
    public interface IFlightPlanService
    {
        Task<int> CreateFlightPlan(FlightPlanRequest request);
        Task<FlightPlanDto> GetFlightPlan(int id);
        Task<List<FlightPlanDto>> GetFlightPlansForUser(string userId);
        Task<PagedFlightPlans> GetFlightPlansPaged(PagedRequest request);
    }
}