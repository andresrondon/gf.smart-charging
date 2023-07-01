using Microsoft.Extensions.Options;
using SmartCharging.Lib.Models;

namespace SmartCharging.Lib.Repositories.Connectors;

public class ConnectorRepository : Repository<Connector>, IConnectorRepository
{
    public ConnectorRepository(IOptions<DatabaseSettings> databaseSettings) : base(databaseSettings.Value, "Connectors")
    {
    }
}
