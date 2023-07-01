using SmartCharging.Lib.Models;

namespace SmartCharging.Lib.Services.Connectors;

public interface IConnectorService
{
    Task AddAsync(Connector connector);
    Task<Connector?> FindAsync(string id, string stationId);
    Task UpdateAsync(Connector connector);
    Task DeleteAsync(string id, string stationId);
}
