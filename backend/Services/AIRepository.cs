using backend.Data;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class AIRepository : IAIRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AIRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<int> AddToDatabaseAsync(string response, int flightPlanId)
        {
            var aiResponse = new AIResponse
            {
                Response = response,
                FlightPlanId = flightPlanId
            };

            _context.AIResponses.Add(aiResponse);

            await _context.SaveChangesAsync();

            return aiResponse.Id;
        }

        public async Task<AIResponse?> GetAIResponseByFlightPlanId(int flightPlanId)
        {
            var aiResponse = await _context.AIResponses
                .Where(a => a.FlightPlanId == flightPlanId)
                .FirstOrDefaultAsync();

            if (aiResponse == null)
            {
                return null;
            }

            return new AIResponse
            {
                Id = aiResponse.Id,
                Response = aiResponse.Response,
                FlightPlanId = aiResponse.FlightPlanId
            };
        }
    }
}