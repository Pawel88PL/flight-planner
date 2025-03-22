using backend.Data;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core; // <-- Konieczne do dynamicznej zmiany sortowania

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

        public async Task<Aircraft?> GetAircraftById(int id)
        {
            var aircraft = await _context.Aircrafts.FindAsync(id);

            if (aircraft == null)
            {
                return null;
            }

            return aircraft;
        }

        public async Task<IEnumerable<Aircraft>> GetAircrafts()
        {
            var aircrafts = await _context.Aircrafts.ToListAsync();
            
            if (aircrafts == null)
            {
                return new List<Aircraft>();
            }

            return aircrafts;
        }

        public async Task<PagedAircrafts> GetAircraftsPaged(PagedRequest request)
        {
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize < 1) request.PageSize = 10;

            var query = _context.Aircrafts.AsQueryable();

            if (!string.IsNullOrEmpty(request.SearchQuery))
            {
                query = query.Where(c =>
                    c.Name.Contains(request.SearchQuery) ||
                    c.Manufacturer.Contains(request.SearchQuery) ||
                    c.Model.Contains(request.SearchQuery));
            }

            if (!string.IsNullOrEmpty(request.SortColumn))
            {
                var sortExpression = $"{request.SortColumn} {(request.SortDirection == "desc" ? "descending" : "ascending")}";
                query = query.OrderBy(sortExpression);
            }

            int totalRecords = await query.CountAsync();

            var aircrafts = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PagedAircrafts
            {
                TotalRecords = totalRecords,
                Data = aircrafts
            };
        }
    }
}