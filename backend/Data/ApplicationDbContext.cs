using backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Aircraft> Aircrafts { get; set; } = default!;
        public DbSet<AIResponse> AIResponses { get; set; } = default!;
        public DbSet<ArrivalAirport> ArrivalAirports { get; set; } = default!;
        public DbSet<DepartureAirport> DepartureAirports { get; set; } = default!;
        public DbSet<FlightPlanResponse> FlightPlanResponses { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

        }
    }
}