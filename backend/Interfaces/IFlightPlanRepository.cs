using backend.Models;

namespace backend.Interfaces
{
    public interface IFlightPlanRepository
    {
        Task<int> AddFlightPlanAsync(FlightPlanRequest flightPlanRequest);
        Task<FlightPlan> GetFlightPlan(int id);
    }
}