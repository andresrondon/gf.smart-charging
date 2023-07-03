using SmartCharging.Lib.Constants;
using SmartCharging.Lib.Models;
using SmartCharging.Lib.Repositories.ChargeStations;
using SmartCharging.Lib.Repositories.Groups;

namespace SmartCharging.Lib.Services.ChargeStations;

public class ChargeStationService : IChargeStationService
{
    private readonly IChargeStationRepository stationRepository;
    private readonly IGroupRepository groupRepository;

    public ChargeStationService(IChargeStationRepository stationRepository, IGroupRepository groupRepository)
    {
        this.stationRepository = stationRepository;
        this.groupRepository = groupRepository;
    }

    public async Task AddAsync(ChargeStation station)
    {
        // Validate
        var group = await groupRepository.FindAsync(station.GroupId, Defaults.Location);
        BusinessRules
            .ValidateChargeStationUpdate(station, group)
            .ThrowIfInValid();

        // Add resource
        await stationRepository.AddAsync(station);
    }

    public Task DeleteAsync(string groupId, string id)
    {
        return stationRepository.DeleteAsync(id, groupId);
    }

    public Task<ChargeStation> FindAsync(string groupId, string id)
    {
        return stationRepository.FindAsync(id, groupId);
    }

    public async Task UpdateAsync(ChargeStation station)
    {
        // Validate
        var group = await groupRepository.FindAsync(station.GroupId, Defaults.Location);
        BusinessRules
            .ValidateChargeStationUpdate(station, group)
            .ThrowIfInValid();
        
        // Update
        await stationRepository.UpdateAsync(station);
    }
}
