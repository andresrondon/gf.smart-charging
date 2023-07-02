using SmartCharging.Lib.Exceptions;
using SmartCharging.Lib.Models;
using SmartCharging.Lib.Repositories.ChargeStations;
using SmartCharging.Lib.Repositories.Groups;

namespace SmartCharging.Lib.Services.Connectors;

public class ConnectorService : IConnectorService
{
    private readonly IChargeStationRepository stationRepository;
    private readonly IGroupRepository groupRepository;

    public ConnectorService(IChargeStationRepository stationRepository, IGroupRepository groupRepository)
    {
        this.stationRepository = stationRepository;
        this.groupRepository = groupRepository;
    }

    public async Task AddAsync(string location, string groupId, string stationId, Connector connector)
    {
        // TODO: to refactor
        var parentGroup = await groupRepository.FindAsync(groupId, location) ?? throw new NotFoundException("Group not found.");
        parentGroup.ChargeStations = await stationRepository.FindAllByGroupIdAsync(groupId);
        var parentStation = parentGroup.ChargeStations.FirstOrDefault(cs => cs.Id == stationId) ?? throw new NotFoundException("Charge Station not found.");
        // ---

        parentStation.Connectors.Add(connector);

        BusinessRules
            .ValidateConnectorUpdate(connector, parentGroup, parentStation)
            .ThrowIfInValid();

        await stationRepository.UpdateAsync(parentStation!);
    }

    public async Task DeleteAsync(string groupId, string stationId, int connectorId)
    {
        var parentStation = await stationRepository.FindAsync(stationId, groupId) ?? throw new NotFoundException("Charge Station not found.");
        var connector = parentStation.Connectors.FirstOrDefault(cs => cs.Id == connectorId) ?? throw new NotFoundException("Connector not found.");
        parentStation.Connectors.Remove(connector);
        await stationRepository.UpdateAsync(parentStation);
    }

    public async Task<Connector?> FindAsync(string groupId, string stationId, int connectorId)
    {
        var parentStation = await stationRepository.FindAsync(stationId, groupId);
        return parentStation?.Connectors.FirstOrDefault(cs => cs.Id == connectorId);
    }

    public async Task UpdateAsync(string location, string groupId, string stationId, Connector connector)
    {
        // TODO: to refactor
        var parentGroup = await groupRepository.FindAsync(groupId, location) ?? throw new NotFoundException("Group not found.");
        parentGroup.ChargeStations = await stationRepository.FindAllByGroupIdAsync(groupId);
        var parentStation = parentGroup.ChargeStations.FirstOrDefault(cs => cs.Id == stationId) ?? throw new NotFoundException("Charge Station not found.");
        // ---
        
        var oldConnector = parentStation.Connectors.FirstOrDefault(c => c.Id == connector.Id) ?? throw new NotFoundException("Connector not found.");

        parentStation.Connectors.Remove(oldConnector);
        parentStation.Connectors.Add(connector);

        BusinessRules
            .ValidateConnectorUpdate(connector, parentGroup, parentStation)
            .ThrowIfInValid();

        await stationRepository.UpdateAsync(parentStation!);
    }
}
