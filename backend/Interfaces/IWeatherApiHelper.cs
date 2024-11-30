namespace backend.Interfaces
{
    public interface IWeatherApiHelper
    {
        Task<T> GetAsync<T>(string metarOrTaf, string departureICAO, string arrivalICAO);
    }
}