using SmartCharging.Lib.Models;

namespace SmartCharging.Lib.Repositories.ChargeStations;

public interface IChargeStationRepository
{
    Task AddAsync(ChargeStation station);
    Task<ChargeStation> FindAsync(string id, string partitionKey);
    Task UpdateAsync(ChargeStation station);
    Task DeleteAsync(string id, string partitionKey);
}
