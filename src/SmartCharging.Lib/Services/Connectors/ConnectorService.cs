using SmartCharging.Lib.Models;
using SmartCharging.Lib.Repositories.Connectors;

namespace SmartCharging.Lib.Services.Connectors;

public class ConnectorService : IConnectorService
{
    private readonly IConnectorRepository connectorRepository;

    public ConnectorService(IConnectorRepository connectorRepository)
    {
        this.connectorRepository = connectorRepository;
    }

    public Task AddAsync(Connector connector)
    {
        return connectorRepository.AddAsync(connector);
    }

    public Task DeleteAsync(string id, string stationId)
    {
        return connectorRepository.DeleteAsync(id, stationId);
    }

    public Task<Connector?> FindAsync(string id, string stationId)
    {
        return connectorRepository.FindAsync(id, stationId);
    }

    public Task UpdateAsync(Connector connector)
    {
        return connectorRepository.UpdateAsync(connector);
    }
}
