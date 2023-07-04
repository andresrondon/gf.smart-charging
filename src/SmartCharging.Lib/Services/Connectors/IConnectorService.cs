using SmartCharging.Lib.Models;

namespace SmartCharging.Lib.Services.Connectors;

/// <summary>
/// Specifies the contract for a service class in charge of performing Business Logic related to <see cref="Connector"/>s.
/// </summary>
public interface IConnectorService
{
    /// <summary>
    /// Adds a new <see cref="Connector"/>.
    /// </summary>
    Task AddAsync(Connector connector, string groupId, string stationId);

    /// <summary>
    /// Finds a specific <see cref="Connector"/>.
    /// </summary>
    /// <param name="groupId">The Id of the <see cref="Group"/> the station belongs to.</param>
    /// <param name="stationId">The Id of the <see cref="ChargeStation"/> the connector belongs to.</param>
    /// <param name="connectorId">Entity's Primary Key</param>
    Task<Connector> FindAsync(string groupId, string stationId, int connectorId);

    /// <summary>
    /// Updates a <see cref="Connector"/>.
    /// </summary>
    Task UpdateAsync(Connector connector, string groupId, string stationId);

    /// <summary>
    /// Deletes a <see cref="Connector"/>.
    /// </summary>
    /// <param name="groupId">The Id of the <see cref="Group"/> the station belongs to.</param>
    /// <param name="stationId">The Id of the <see cref="ChargeStation"/> the connector belongs to.</param>
    /// <param name="connectorId">Entity's Primary Key</param>
    Task DeleteAsync(string groupId, string stationId, int connectorId);
}
