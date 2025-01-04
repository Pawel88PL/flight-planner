namespace backend.Models
{
    public class FlightPlanRequest
    {   
        public required string DepartureICAO { get; set; }
        public required string ArrivalICAO { get; set; }
        public required string DepartureTime { get; set; }
        public required string FlightDay { get; set; }
        public required string FlightDuration { get; set; }
        public required string UserId { get; set; }
        public int AircraftId { get; set; }
    }
}