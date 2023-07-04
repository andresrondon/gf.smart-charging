using SmartCharging.Lib.Constants;
using SmartCharging.Lib.Models;
using SmartCharging.Lib.Repositories.ChargeStations;
using SmartCharging.Lib.Repositories.Groups;

namespace SmartCharging.Lib.Services.ChargeStations;

/// <summary>
/// A service class in charge of performing Business Logic related to <see cref="ChargeStation"/>s.
/// </summary>
public class ChargeStationService : IChargeStationService
{
    private readonly IChargeStationRepository stationRepository;
    private readonly IGroupRepository groupRepository;

    public ChargeStationService(IChargeStationRepository stationRepository, IGroupRepository groupRepository)
    {
        this.stationRepository = stationRepository;
        this.groupRepository = groupRepository;
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public Task DeleteAsync(string groupId, string id)
    {
        return stationRepository.DeleteAsync(id, groupId);
    }

    /// <inheritdoc/>
    public Task<ChargeStation> FindAsync(string groupId, string id)
    {
        return stationRepository.FindAsync(id, groupId);
    }

    /// <inheritdoc/>
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
