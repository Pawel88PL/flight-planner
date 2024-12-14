using backend.Models;

namespace backend.Interfaces
{
    public interface IWeatherRepository
    {
        Task AddArrivalAndDepartureWeather(WeatherResponse weatherResponse, List<int> airportsIds);
    }
}