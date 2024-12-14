using backend.Models;

namespace backend.Interfaces
{
    public interface IFlightPlanRepository
    {
        Task<int> AddFlightPlanAsync(FlightPlanRequest flightPlanRequest, List<int> airports);
        Task<FlightPlan> GetFlightPlan(int id);
    }
}