namespace backend.Models
{
    public class ArrivalAirport
    {
        public int Id { get; set; }
        
        public string ICAO { get; set; } = string.Empty;
        
        public string Name { get; set; } = string.Empty;
        
        public string City { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;
        
        public string METAR { get; set; } = string.Empty;

        public string TAF { get; set; } = string.Empty;
    }
}