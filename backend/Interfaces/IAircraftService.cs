using backend.Models;

namespace backend.Interfaces
{
    public interface IAircraftService
    {
        Task AddAircraft(Aircraft aircraft);
    }
}