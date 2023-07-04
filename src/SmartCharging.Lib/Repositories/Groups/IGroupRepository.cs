using SmartCharging.Lib.Models;

namespace SmartCharging.Lib.Repositories.Groups;

/// <summary>
/// Specifies the contract for a repository that performs CRUD operations on <see cref="Group"/>s.
/// </summary>
public interface IGroupRepository
{
    /// <summary>
    /// Adds a new <see cref="Group"/>.
    /// </summary>
    Task<Group> AddAsync(Group group);

    /// <summary>
    /// Finds a specific <see cref="Group"/>.
    /// </summary>
    /// <param name="id">Entity's Primary Key</param>
    /// <param name="locationArea">Entity's Partition Key</param>
    Task<Group> FindAsync(string id, string locationArea);

    /// <summary>
    /// Updates a <see cref="Group"/>.
    /// </summary>
    Task<Group> UpdateAsync(Group group);

    /// <summary>
    /// Deletes a <see cref="Group"/>.
    /// </summary>
    /// <param name="id">Entity's Primary Key</param>
    /// <param name="locationArea">Entity's Partition Key</param>
    Task DeleteAsync(string id, string locationArea);
}
