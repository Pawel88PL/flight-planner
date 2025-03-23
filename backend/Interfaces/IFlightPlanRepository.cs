using backend.Models;

namespace backend.Interfaces
{
    public interface IFlightPlanRepository
    {
        Task<int> AddFlightPlanAsync(FlightPlanRequest flightPlanRequest, List<int> airports);
        Task DeleteFlightPlan(int id);
        Task<FlightPlan> GetFlightPlan(int id);
        Task<List<FlightPlan>> GetFlightPlansForUser(string userId);
        Task<PagedFlightPlans> GetFlightPlansPaged(PagedRequest request);
    }
}