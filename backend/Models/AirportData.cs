namespace backend.Models
{
    public class AirportData
    {
        public string ICAO { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public Country Country { get; set; } = new Country();
        public string Location { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
    
    public class Country
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class AirportsResponse
    {
        public int Results { get; set; }
        public List<AirportData> Data { get; set; } = new List<AirportData>();
    }
}