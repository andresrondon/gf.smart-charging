using SmartCharging.Lib.Models;

namespace SmartCharging.Lib.Repositories.ChargeStations;

/// <summary>
/// Specifies the contract for a repository that performs CRUD operations on <see cref="ChargeStation"/>s.
/// </summary>
public interface IChargeStationRepository
{
    /// <summary>
    /// Adds a new <see cref="ChargeStation"/>.
    /// </summary>
    Task<ChargeStation> AddAsync(ChargeStation station);

    /// <summary>
    /// Finds a specific <see cref="ChargeStation"/>.
    /// </summary>
    /// <param name="id">Entity's Primary Key</param>
    /// <param name="groupId">Entity's Partition Key</param>
    Task<ChargeStation> FindAsync(string id, string groupId);

    /// <summary>
    /// Updates a <see cref="ChargeStation"/>.
    /// </summary>
    Task<ChargeStation> UpdateAsync(ChargeStation station);

    /// <summary>
    /// Deletes a <see cref="ChargeStation"/>.
    /// </summary>
    /// <param name="id">Entity's Primary Key</param>
    /// <param name="groupId">Entity's Partition Key</param>
    Task DeleteAsync(string id, string groupId);

    /// <summary>
    /// Deletes all <see cref="ChargeStation"/>s belonging to a groupId.
    /// </summary>
    Task BulkDeleteAsync(string groupId);
}
