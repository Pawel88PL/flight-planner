using backend.Models;

namespace backend.Interfaces
{
    public interface IAirportService
    {
        Task<List<AirportData>> GetDepartureAndArrivalAirports(string departureICAO, string arrivalICAO);
    }
}