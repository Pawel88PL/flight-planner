using backend.Data;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;
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
                    FlightDay = flightPlanRequest.FlightDay,
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
    }
}