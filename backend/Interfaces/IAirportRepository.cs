using backend.Models;

namespace backend.Interfaces
{
    public interface IAirportRepository
    {
        Task<List<int>> AddAirportsToDatabase(List<AirportData> airports);
        Task<ArrivalAirport?> GetArrivalAirportByICAO(string icao);
        Task<DepartureAirport?> GetDepartureAirportByICAO(string icao);
    }
}