using backend.Models;

namespace backend.Interfaces
{
    public interface IAirportService
    {
        Task GetDepartureAndArrivalAirports(string departureICAO, string arrivalICAO);
    }
}