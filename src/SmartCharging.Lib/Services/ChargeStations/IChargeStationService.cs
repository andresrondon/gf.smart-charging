using SmartCharging.Lib.Models;

namespace SmartCharging.Lib.Services.ChargeStations;

public interface IChargeStationService
{
    Task AddAsync(string location, ChargeStation station);
    Task<ChargeStation> FindAsync(string groupId, string stationId);
    Task UpdateAsync(string location, ChargeStation station);
    Task DeleteAsync(string groupId, string stationId);
}
