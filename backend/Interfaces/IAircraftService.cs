using backend.Models;

namespace backend.Interfaces
{
    public interface IAircraftService
    {
        Task AddAircraft(Aircraft aircraft);
        Task DeleteAircraft(int id);
        Task EditAircraft(Aircraft aircraft);
        Task<Aircraft?> GetAircraftById(int id);
        Task<IEnumerable<Aircraft>> GetAircrafts();
        Task<PagedAircrafts> GetAircraftsPaged(PagedRequest request);
    }
}