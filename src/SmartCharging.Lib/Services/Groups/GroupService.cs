using SmartCharging.Lib.Constants;
using SmartCharging.Lib.Models;
using SmartCharging.Lib.Repositories.ChargeStations;
using SmartCharging.Lib.Repositories.Groups;

namespace SmartCharging.Lib.Services.Groups;

/// <summary>
/// A service class in charge of performing Business Logic related to <see cref="Group"/>s.
/// </summary>
public class GroupService : IGroupService
{
    private readonly IGroupRepository groupRepository;
    private readonly IChargeStationRepository stationRepository;

    public GroupService(IGroupRepository groupRepository, IChargeStationRepository stationRepository)
    {
        this.groupRepository = groupRepository;
        this.stationRepository = stationRepository;
    }

    /// <inheritdoc/>
    public Task AddAsync(Group group)
    {
        BusinessRules
            .ValidateGroupUpdate(group)
            .ThrowIfInValid();

        return groupRepository.AddAsync(group);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(string id)
    {
        await stationRepository.BulkDeleteAsync(id);
        await groupRepository.DeleteAsync(id, Defaults.Location);
    }

    /// <inheritdoc/>
    public Task<Group> FindAsync(string id)
    {
        return groupRepository.FindAsync(id, Defaults.Location);
    }

    /// <inheritdoc/>
    public Task UpdateAsync(Group group)
    {
        BusinessRules
            .ValidateGroupUpdate(group)
            .ThrowIfInValid();

        return groupRepository.UpdateAsync(group);
    }
}
