namespace backend.Models
{
    public class AIResponse
    {
        public int Id { get; set; }
        public string? Response { get; set; }

        public int FlightPlanId { get; set; }
        public FlightPlan FlightPlan { get; set; } = default!;
    }

    public class AIResponseDto
    {
        public string? Response { get; set; }
    }
}