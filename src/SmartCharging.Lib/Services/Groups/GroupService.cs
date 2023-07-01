using SmartCharging.Lib.Models;
using SmartCharging.Lib.Repositories.ChargeStations;
using SmartCharging.Lib.Repositories.Connectors;
using SmartCharging.Lib.Repositories.Groups;

namespace SmartCharging.Lib.Services.Groups;

public class GroupService : IGroupService
{
    private readonly IGroupRepository groupRepository;
    private readonly IChargeStationRepository stationRepository;
    private readonly IConnectorRepository connectorRepository;

    public GroupService(IGroupRepository groupRepository, IChargeStationRepository stationRepository, IConnectorRepository connectorRepository)
    {
        this.groupRepository = groupRepository;
        this.stationRepository = stationRepository;
        this.connectorRepository = connectorRepository;
    }

    public Task AddAsync(Group group)
    {
        return groupRepository.AddAsync(group);
    }

    public Task DeleteAsync(string id)
    {
        return groupRepository.DeleteAsync(id, Constants.Defaults.PartitionKey);
    }

    public async Task<Group?> FindAsync(string id)
    {
        var group = await groupRepository.FindAsync(id, Constants.Defaults.PartitionKey);
        
        // Populate child entities
        group.ChargeStations = await stationRepository.FindAllByGroupId(id);
        foreach (var station in group.ChargeStations)
        {
            station.Connectors = await connectorRepository.FindAllByChargeStationId(station.Id);
        }
        
        return group;
    }

    public Task UpdateAsync(Group group)
    {
        return groupRepository.UpdateAsync(group);
    }
}
