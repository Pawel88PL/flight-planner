using backend.Models;

namespace backend.Interfaces
{
    public interface IAIService
    {
        Task<string> CreateJustification(WeatherResponse weatherResponse);
    }
}