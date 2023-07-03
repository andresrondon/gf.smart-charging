using SmartCharging.Lib.Constants;
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

    public async Task AddAsync(Connector connector, string groupId, string stationId)
    {
        var parentGroup = await groupRepository.FindAsync(groupId, Defaults.Location);
        var parentStation = GetStationOrThrow(parentGroup, stationId);

        parentStation.Connectors.Add(connector);

        BusinessRules
            .ValidateConnectorUpdate(connector, parentGroup, parentStation)
            .ThrowIfInValid();

        await stationRepository.UpdateAsync(parentStation!);
    }

    public async Task DeleteAsync(string groupId, string stationId, int connectorId)
    {
        var parentStation = await stationRepository.FindAsync(stationId, groupId);
        var connector = GetConnectorOrThrow(parentStation, connectorId);
        parentStation.Connectors.Remove(connector);

        await stationRepository.UpdateAsync(parentStation);
    }

    public async Task<Connector> FindAsync(string groupId, string stationId, int connectorId)
    {
        var parentStation = await stationRepository.FindAsync(stationId, groupId);
        return GetConnectorOrThrow(parentStation, connectorId);
    }

    public async Task UpdateAsync(Connector connector, string groupId, string stationId)
    {
        var parentGroup = await groupRepository.FindAsync(groupId, Defaults.Location);
        var parentStation = GetStationOrThrow(parentGroup, stationId);
        var oldConnector = GetConnectorOrThrow(parentStation, connector.Id);

        parentStation.Connectors.Remove(oldConnector);
        parentStation.Connectors.Add(connector);

        BusinessRules
            .ValidateConnectorUpdate(connector, parentGroup, parentStation)
            .ThrowIfInValid();

        await stationRepository.UpdateAsync(parentStation!);
    }

    private static ChargeStation GetStationOrThrow(Group parentGroup, string stationId)
        => parentGroup.ChargeStations.FirstOrDefault(cs => cs.Id == stationId) ?? throw new ResourceNotFoundException("Charge Station not found.");

    private static Connector GetConnectorOrThrow(ChargeStation parentStation, int connectorId) 
        => parentStation.Connectors.FirstOrDefault(c => c.Id == connectorId) ?? throw new ResourceNotFoundException("Connector not found.");
}
