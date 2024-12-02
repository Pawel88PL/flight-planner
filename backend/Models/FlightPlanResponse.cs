using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class FlightPlanResponse
    {
        public int Id { get; set; }

        // Informacje o lotniskach
        [Required(ErrorMessage = "Departure ICAO code is required")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Departure ICAO code must be exactly 4 characters")]
        [RegularExpression(@"^[A-Z]{4}$", ErrorMessage = "Departure ICAO code must contain only uppercase letters")]
        public string DepartureICAO { get; set; } = string.Empty;

        [Required(ErrorMessage = "Arrival ICAO code is required")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Arrival ICAO code must be exactly 4 characters")]
        [RegularExpression(@"^[A-Z]{4}$", ErrorMessage = "Arrival ICAO code must contain only uppercase letters")]
        public string ArrivalICAO { get; set; } = string.Empty;

        // Informacje o czasie
        [Required(ErrorMessage = "Departure time is required")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Departure time must be exactly 4 digits")]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "Departure time must be in the format HHMM")]
        public string DepartureTime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Flight day is required")]
        [StringLength(8, MinimumLength = 5, ErrorMessage = "Flight day must be either 'today' or 'tomorrow'")]
        [RegularExpression(@"^(today|tomorrow)$", ErrorMessage = "Flight day must be either 'today' or 'tomorrow'")]
        public string FlightDay { get; set; } = string.Empty;

        [Required(ErrorMessage = "Flight duration is required")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Flight duration must be exactly 4 digits")]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "Flight duration must be in the format HHMM")]
        public string FlightDuration { get; set; } = string.Empty;

        // Informacje o samolocie
        [Required(ErrorMessage = "Aircraft ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Aircraft ID must be a positive integer")]
        public int AircraftId { get; set; }
        
        // Metar dla lotniska wylotowego
        [Required(ErrorMessage = "Departure METAR is required")]
        public string DepartureMETAR { get; set; } = string.Empty;

        // TAF dla lotniska wylotowego
        [Required(ErrorMessage = "Departure TAF is required")] 
        public string DepartureTAF { get; set; } = string.Empty;
        
        // Metar dla lotniska przylotowego
        [Required(ErrorMessage = "Arrival METAR is required")]
        public string ArrivalMETAR { get; set; } = string.Empty;

        // TAF dla lotniska przylotowego
        [Required(ErrorMessage = "Arrival TAF is required")]
        public string ArrivalTAF { get; set; } = string.Empty;

        // Nazwa lotniska wylotowego
        public string DepartureAirportName { get; set; } = string.Empty;

        // Miasto lotniska wylotowego
        public string DepartureCity { get; set; } = string.Empty;

        // Kraj lotniska wylotowego
        public string DepartureCountry { get; set; } = string.Empty;

        // Nazwa lotniska przylotowego
        public string ArrivalAirportName { get; set; } = string.Empty;

        // Miasto lotniska przylotowego
        public string ArrivalCity { get; set; } = string.Empty;

        // Kraj lotniska przylotowego
        public string ArrivalCountry { get; set; } = string.Empty;
    }

    public class FlightPlanResponseDto
    {
        public string FlightDay { get; set; } = string.Empty;
        public string FlightDuration { get; set; } = string.Empty;
        public string DepartureTime { get; set; } = string.Empty;
        public string DepartureICAO { get; set; } = string.Empty;
        public string DepartureMETAR { get; set; } = string.Empty;
        public string DepartureTAF { get; set; } = string.Empty;
        public string ArrivalICAO { get; set; } = string.Empty;
        public string ArrivalMETAR { get; set; } = string.Empty;
        public string ArrivalTAF { get; set; } = string.Empty;
        public int AircraftId { get; set; }

        public string DepartureAirportName { get; set; } = string.Empty;
        public string DepartureCity { get; set; } = string.Empty;
        public string DepartureCountry { get; set; } = string.Empty;

        public string ArrivalAirportName { get; set; } = string.Empty;
        public string ArrivalCity { get; set; } = string.Empty;
        public string ArrivalCountry { get; set; } = string.Empty;
    }
}