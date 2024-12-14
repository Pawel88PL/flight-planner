using backend.Data;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class FlightPlanRepository : IFlightPlanRepository
    {
        private readonly ApplicationDbContext _context;

        public FlightPlanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddFlightPlanAsync(FlightPlanRequest flightPlanRequest)
        {
            var newFlightPlan = new FlightPlan
            {
                DepartureICAO = flightPlanRequest.DepartureICAO,
                ArrivalICAO = flightPlanRequest.ArrivalICAO,
                DepartureTime = flightPlanRequest.DepartureTime,
                FlightDay = flightPlanRequest.FlightDay,
                FlightDuration = flightPlanRequest.FlightDuration,
                AircraftId = flightPlanRequest.AircraftId,
            };

            _context.FlightPlans.Add(newFlightPlan);

            await _context.SaveChangesAsync();

            return newFlightPlan.Id;
        }

        public async Task<FlightPlan> GetFlightPlan(int id)
        {
            var flightPlan = await _context.FlightPlans
                .Where(f => f.Id == id)
                .Include(dep => dep.DepartureAirport)
                .FirstOrDefaultAsync();

            if (flightPlan == null)
            {
                throw new Exception("Flight plan request not found.");
            }

            return flightPlan;
        }
    }
}