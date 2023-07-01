using SmartCharging.Lib.Models;

namespace SmartCharging.Lib.Repositories.Connectors;

public interface IConnectorRepository
{
    Task AddAsync(Connector connector);
    Task<Connector?> FindAsync(string id, string partitionKey);
    Task UpdateAsync(Connector connector);
    Task DeleteAsync(string id, string partitionKey);
}
