using SmartCharging.Lib.Models;

namespace SmartCharging.Lib.Services.Connectors;

public interface IConnectorService
{
    Task AddAsync(string groupId, string stationId, Connector connector);
    Task<Connector> FindAsync(string groupId, string stationId, int connectorId);
    Task UpdateAsync(string groupId, string stationId, Connector connector);
    Task DeleteAsync(string groupId, string stationId, int connectorId);
}
