namespace backend.Models
{
    public class FlightPlanResponse
    {
        public int ResponseId { get; set; }
        public string? DepartureICAO { get; set; }
        public string? ArrivalICAO { get; set; }
        public string? DepartureTime { get; set; }
        public string? FlightDay { get; set; }
        public string? FlightDuration { get; set; }
        public string? AircraftId { get; set; }
        public bool FetchWeatherData { get; set; }
        public string? DepartureMETAR { get; set; }
        public string? ArrivalMETAR { get; set; }
        public string? DepartureTAF { get; set; }
        public string? ArrivalTAF { get; set; }
        public string? AircraftType { get; set; }
    }
}