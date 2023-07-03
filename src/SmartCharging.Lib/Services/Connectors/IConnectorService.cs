using SmartCharging.Lib.Models;

namespace SmartCharging.Lib.Services.Connectors;

public interface IConnectorService
{
    Task AddAsync(Connector connector, string groupId, string stationId);
    Task<Connector> FindAsync(string groupId, string stationId, int connectorId);
    Task UpdateAsync(Connector connector, string groupId, string stationId);
    Task DeleteAsync(string groupId, string stationId, int connectorId);
}
