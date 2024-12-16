using Newtonsoft.Json;

namespace backend.Models
{
    public class WeatherResponse
    {
        public string DepartureMETAR { get; set; } = string.Empty;
        public string ArrivalMETAR { get; set; } = string.Empty;
        public string DepartureTAF { get; set; } = string.Empty;
        public string ArrivalTAF { get; set; } = string.Empty;
    }

    public class WeatherData
    {
        [JsonProperty("metar_id")]
        public int MetarId { get; set; }

        [JsonProperty("icaoId")]
        public string? ICAO { get; set; }

        [JsonProperty("rawOb")]
        public string? RawMetar { get; set; } // Dane dla METAR

        [JsonProperty("rawTaf")]
        public string? RawTaf { get; set; } // Dane dla TAF
    }
}