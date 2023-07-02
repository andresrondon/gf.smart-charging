using SmartCharging.Lib.Models;
using SmartCharging.Lib.Repositories.ChargeStations;
using SmartCharging.Lib.Repositories.Groups;

namespace SmartCharging.Lib.Services.Groups;

public class GroupService : IGroupService
{
    private readonly IGroupRepository groupRepository;
    private readonly IChargeStationRepository stationRepository;

    public GroupService(IGroupRepository groupRepository, IChargeStationRepository stationRepository)
    {
        this.groupRepository = groupRepository;
        this.stationRepository = stationRepository;
    }

    public Task AddAsync(Group group)
    {
        BusinessRules
            .ValidateGroupUpdate(group)
            .ThrowIfInValid();

        return groupRepository.AddAsync(group);
    }

    public async Task DeleteAsync(string location, string id)
    {
        await stationRepository.BulkDeleteAsync(id);
        await groupRepository.DeleteAsync(id, location);
    }

    public async Task<Group?> FindAsync(string location, string id)
    {
        var group = await groupRepository.FindAsync(id, location);

        if (group is not null)
        {
            group.ChargeStations = await stationRepository.FindAllByGroupIdAsync(id);
        }

        return group;
    }

    public Task UpdateAsync(Group group)
    {
        BusinessRules
            .ValidateGroupUpdate(group)
            .ThrowIfInValid();

        return groupRepository.UpdateAsync(group);
    }
}
