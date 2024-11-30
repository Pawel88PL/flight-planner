using backend.Models;

namespace backend.Interfaces
{
    public interface IWeatherService
    {
        Task<WeatherResponse> GetWeatherDataForDepartureAndArrival(string departureICAO, string arrivalICAO);
    }
}