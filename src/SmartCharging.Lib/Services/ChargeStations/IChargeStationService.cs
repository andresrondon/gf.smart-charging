using SmartCharging.Lib.Models;

namespace SmartCharging.Lib.Services.ChargeStations;

/// <summary>
/// Specifies the contract for a service class in charge of performing Business Logic related to <see cref="ChargeStation"/>s.
/// </summary>
public interface IChargeStationService
{
    /// <summary>
    /// Adds a new <see cref="ChargeStation"/>.
    /// </summary>
    Task AddAsync(ChargeStation station);

    /// <summary>
    /// Finds a specific <see cref="ChargeStation"/>.
    /// </summary>
    /// <param name="groupId">The Id of the <see cref="Group"/> the station belongs to.</param>
    /// <param name="stationId">Entity's Primary Key</param>
    Task<ChargeStation> FindAsync(string groupId, string stationId);

    /// <summary>
    /// Updates a <see cref="ChargeStation"/>.
    /// </summary>
    Task UpdateAsync(ChargeStation station);

    /// <summary>
    /// Deletes a <see cref="ChargeStation"/>.
    /// </summary>
    /// <param name="groupId">The Id of the <see cref="Group"/> the station belongs to.</param>
    /// <param name="stationId">Entity's Primary Key</param>
    Task DeleteAsync(string groupId, string stationId);
}
