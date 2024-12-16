using Newtonsoft.Json;

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
        [JsonProperty("data")]
        public int Id { get; set; }

        [JsonProperty("icaoId")]
        public string? ICAO { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("country")]
        public string? Country { get; set; }
    }
}