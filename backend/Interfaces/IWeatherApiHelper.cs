namespace backend.Interfaces
{
    public interface IWeatherApiHelper
    {
        Task<T> GetAsync<T>(string departureICAO, string arrivalICAO);
    }
}