using SmartCharging.Lib.Models;

namespace SmartCharging.Lib.Repositories.ChargeStations;

public interface IChargeStationRepository
{
    Task<ChargeStation> AddAsync(ChargeStation station);
    Task<ChargeStation> FindAsync(string id, string groupId);
    Task<ChargeStation> UpdateAsync(ChargeStation station);
    Task DeleteAsync(string id, string groupId);
    Task BulkDeleteAsync(string groupId);
}
