using SmartCharging.Lib.Exceptions;
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

    public async Task AddAsync(string location, ChargeStation station)
    {
        _ = await groupRepository.FindAsync(station.GroupId, location) ?? throw new NotFoundException("Group not found.");
        await stationRepository.AddAsync(station);
    }

    public Task DeleteAsync(string groupId, string id)
    {
        return stationRepository.DeleteAsync(id, groupId);
    }

    public Task<ChargeStation?> FindAsync(string groupId, string id)
    {
        return stationRepository.FindAsync(id, groupId);
    }

    public Task UpdateAsync(ChargeStation station)
    {
        return stationRepository.UpdateAsync(station);
    }
}
