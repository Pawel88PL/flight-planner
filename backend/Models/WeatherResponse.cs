namespace backend.Models
{
    public class WeatherResponse
    {
        public string DepartureMETAR { get; set; } = string.Empty;
        public string ArrivalMETAR { get; set; } = string.Empty;
        public string DepartureTAF { get; set; } = string.Empty;
        public string ArrivalTAF { get; set; } = string.Empty;
    }

    public class MetarResponse
    {
        public int Results { get; set; }
        public List<string> Data { get; set; } = new List<string>();
    }

    public class TafResponse
    {
        public int Results { get; set; }
        public List<string> Data { get; set; } = new List<string>();
    }
}