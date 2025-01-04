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
        public DbSet<FlightPlan> FlightPlans { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FlightPlan>()
                .HasOne(fp => fp.DepartureAirport)
                .WithMany()
                .HasForeignKey(fp => fp.DepartureAirportId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FlightPlan>()
                .HasOne(fp => fp.ArrivalAirport)
                .WithMany()
                .HasForeignKey(fp => fp.ArrivalAirportId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AIResponse>()
                .HasOne(ai => ai.FlightPlan)
                .WithMany(fp => fp.AIResponses)
                .HasForeignKey(ai => ai.FlightPlanId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<FlightPlan>()
                .HasOne(fp => fp.User)
                .WithMany()
                .HasForeignKey(fp => fp.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}