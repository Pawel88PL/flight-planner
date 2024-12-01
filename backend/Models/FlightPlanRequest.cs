namespace backend.Models
{
    public class FlightPlanRequest
    {   
        public string DepartureICAO { get; set; } = string.Empty;
        public string ArrivalICAO { get; set; } = string.Empty;
        public string DepartureTime { get; set; } = string.Empty;
        public string FlightDay { get; set; } = string.Empty;
        public string FlightDuration { get; set; } = string.Empty;
        public int AircraftId { get; set; }
    }
}