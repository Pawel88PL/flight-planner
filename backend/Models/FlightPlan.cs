using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class FlightPlan
    {
        public int Id { get; set; }

        [StringLength(4)]
        public string DepartureTime { get; set; } = string.Empty;

        [StringLength(4)]
        public string FlightDuration { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public int AircraftId { get; set; }

        public int DepartureAirportId { get; set; }
        public DepartureAirport DepartureAirport { get; set; } = default!;

        public int ArrivalAirportId { get; set; }
        public ArrivalAirport ArrivalAirport { get; set; } = default!;

        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = default!;

        public List<AIResponse> AIResponses { get; set; } = new List<AIResponse>();
    }

    public class FlightPlanDto
    {
        public int Id { get; set; }
        public string FlightDuration { get; set; } = string.Empty;
        public string DepartureTime { get; set; } = string.Empty;
        public int AircraftId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DepartureAirport DepartureAirport { get; set; } = new DepartureAirport();
        public ArrivalAirport ArrivalAirport { get; set; } = new ArrivalAirport();
    }
}