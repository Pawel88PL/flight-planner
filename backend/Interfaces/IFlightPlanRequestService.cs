using backend.Models;

namespace backend.Interfaces
{
    public interface IFlightPlanRequestService
    {
        Task<FlightPlanResponse> CreateFlightPlanRequest(FlightPlanRequest flightPlanRequest);
    }
}