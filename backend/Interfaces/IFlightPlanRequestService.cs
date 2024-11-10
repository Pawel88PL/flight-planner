using backend.Models;

namespace backend.Interfaces
{
    public interface IFlightPlanRequestService
    {
        Task AddFlightPlanRequestAsync(FlightPlanRequest flightPlanRequest);
    }
}