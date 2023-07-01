using SmartCharging.Lib.Models;
using SmartCharging.Lib.Repositories.ChargeStations;

namespace SmartCharging.Lib.Services.ChargeStations;

public class ChargeStationService : IChargeStationService
{
    private readonly IChargeStationRepository stationRepository;

    public ChargeStationService(IChargeStationRepository stationRepository)
    {
        this.stationRepository = stationRepository;
    }

    public Task AddAsync(ChargeStation group)
    {
        return stationRepository.AddAsync(group);
    }

    public Task DeleteAsync(string id, string groupId)
    {
        return stationRepository.DeleteAsync(id, Constants.Defaults.PartitionKey);
    }

    public Task<ChargeStation?> FindAsync(string id, string groupId)
    {
        return stationRepository.FindAsync(id, Constants.Defaults.PartitionKey);
    }

    public Task UpdateAsync(ChargeStation group)
    {
        return stationRepository.UpdateAsync(group);
    }

    public Task BulkDelete(string groupId)
    {
        return stationRepository.BulkDelete(groupId);
    }
}
