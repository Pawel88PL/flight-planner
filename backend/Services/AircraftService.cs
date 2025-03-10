using backend.Data;
using backend.Interfaces;
using backend.Models;

namespace backend.Services
{
    public class AircraftService : IAircraftService
    {
        private readonly ApplicationDbContext _context;

        public AircraftService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAircraft(Aircraft aircraft)
        {
            try
            {
                await _context.Aircrafts.AddAsync(aircraft);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}