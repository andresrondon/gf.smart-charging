using SmartCharging.Lib.Models;

namespace SmartCharging.Lib.Services.ChargeStations;

public interface IChargeStationService
{
    Task AddAsync(ChargeStation station);
    Task<ChargeStation?> FindAsync(string id, string groupId);
    Task UpdateAsync(ChargeStation station);
    Task DeleteAsync(string id, string groupId);
    Task BulkDelete(string groupId);
}
