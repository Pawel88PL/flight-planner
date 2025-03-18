using backend.Models;

namespace backend.Interfaces
{
    public interface IAircraftService
    {
        Task AddAircraft(Aircraft aircraft);
        Task<PagedAircrafts> GetAircraftsPaged(PagedRequest request);
    }
}