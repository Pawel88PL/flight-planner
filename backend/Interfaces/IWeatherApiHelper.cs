namespace backend.Interfaces
{
    public interface IWeatherApiHelper
    {
        Task<T> GetAsync<T>(string request, string departureICAO, string arrivalICAO);
    }
}