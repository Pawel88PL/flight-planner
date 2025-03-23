using backend.Data;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core; // <-- Konieczne do dynamicznej zmiany sortowania
using Serilog;

namespace backend.Services
{
    public class FlightPlanRepository : IFlightPlanRepository
    {
        private readonly ApplicationDbContext _context;

        public FlightPlanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddFlightPlanAsync(FlightPlanRequest flightPlanRequest, List<int> airports)
        {
            try
            {
                var newFlightPlan = new FlightPlan
                {
                    DepartureAirportId = airports[0],
                    ArrivalAirportId = airports[1],
                    DepartureTime = flightPlanRequest.DepartureTime,
                    FlightDuration = flightPlanRequest.FlightDuration,
                    AircraftId = flightPlanRequest.AircraftId,
                    UserId = flightPlanRequest.UserId
                };

                _context.FlightPlans.Add(newFlightPlan);

                await _context.SaveChangesAsync();

                return newFlightPlan.Id;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to add flight plan to database.");
                throw new Exception("Failed to add flight plan to database.", ex);
            }
        }

        public async Task DeleteFlightPlan(int id)
        {
            var flightPlan = await _context.FlightPlans.FindAsync(id);

            if (flightPlan == null)
            {
                throw new Exception("Flight plan not found.");
            }

            _context.FlightPlans.Remove(flightPlan);

            await _context.SaveChangesAsync();
        }

        public async Task<FlightPlan> GetFlightPlan(int id)
        {
            var flightPlan = await _context.FlightPlans
                .Where(f => f.Id == id)
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .FirstOrDefaultAsync();

            if (flightPlan == null)
            {
                throw new Exception("Flight plan request not found.");
            }

            return flightPlan;
        }

        public async Task<List<FlightPlan>> GetFlightPlansForUser(string userId)
        {
            var flightPlans = await _context.FlightPlans
                .Where(f => f.UserId == userId)
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .ToListAsync();

            return flightPlans;
        }

        public async Task<PagedFlightPlans> GetFlightPlansPaged(PagedRequest request)
        {
            var query = from plan in _context.FlightPlans.AsNoTracking()
                        join dep in _context.DepartureAirports on plan.DepartureAirportId equals dep.Id
                        join arr in _context.ArrivalAirports on plan.ArrivalAirportId equals arr.Id
                        join aircraft in _context.Aircrafts on plan.AircraftId equals aircraft.Id
                        join user in _context.Users on plan.UserId equals user.Id
                        select new
                        {
                            FlightPlan = plan,
                            DepartureAirport = dep,
                            ArrivalAirport = arr,
                            Aircraft = aircraft,
                            User = user
                        };

            if (!string.IsNullOrEmpty(request.SearchQuery))
            {
                query = query.Where(c =>
                    c.FlightPlan.DepartureAirport.Name!.Contains(request.SearchQuery) ||
                    c.FlightPlan.ArrivalAirport.Name!.Contains(request.SearchQuery) ||
                    c.Aircraft.Name!.Contains(request.SearchQuery) ||
                    c.User.FirstName!.Contains(request.SearchQuery) ||
                    c.User.LastName!.Contains(request.SearchQuery));
            }

            if (!string.IsNullOrEmpty(request.SortColumn))
            {
                var sortExpression = request.SortColumn switch
                {
                    "CreatedAt" => $"FlightPlan.CreatedAt {(request.SortDirection == "desc" ? "descending" : "ascending")}",
                    "DepartureAirport" => $"DepartureAirport.Name {(request.SortDirection == "desc" ? "descending" : "ascending")}",
                    "ArrivalAirport" => $"ArrivalAirport.Name {(request.SortDirection == "desc" ? "descending" : "ascending")}",
                    "Aircraft" => $"Aircraft.Name {(request.SortDirection == "desc" ? "descending" : "ascending")}",
                    "UserFullName" => $"User.FirstName {(request.SortDirection == "desc" ? "descending" : "ascending")}, User.LastName",
                    _ => $"FlightPlan.CreatedAt descending"
                };

                query = query.OrderBy(sortExpression);
            }
            else
            {
                query = query.OrderByDescending(c => c.FlightPlan.CreatedAt);
            }

            int totalRecords = await query.CountAsync();

            var flightPlans = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var flightPlansDTO = flightPlans.Select(item => new FlightPlanListDto
            {
                Id = item.FlightPlan.Id,
                FlightDuration = item.FlightPlan.FlightDuration,
                DepartureTime = item.FlightPlan.DepartureTime,
                AircraftName = item.Aircraft.Name,
                CreatedAt = item.FlightPlan.CreatedAt,
                DepartureAirport = item.DepartureAirport.Name,
                ArrivalAirport = item.ArrivalAirport.Name,
                UserFullName = item.User.FirstName + " " + item.User.LastName
            }).ToList();

            return new PagedFlightPlans
            {
                TotalRecords = totalRecords,
                Data = flightPlansDTO
            };
        }
    }
}