using backend.Models;

namespace backend.Interfaces
{
    public interface IFlightPlanRequestService
    {
        Task<string> CreateFlightPlanRequest(FlightPlanRequest flightPlanRequest);
    }
}