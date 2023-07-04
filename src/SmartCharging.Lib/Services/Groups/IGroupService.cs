using SmartCharging.Lib.Models;

namespace SmartCharging.Lib.Services.Groups;

/// <summary>
/// Specifies the contract for a service class in charge of performing Business Logic related to <see cref="Group"/>s.
/// </summary>
public interface IGroupService
{
    /// <summary>
    /// Adds a new <see cref="Group"/>.
    /// </summary>
    Task AddAsync(Group group);

    /// <summary>
    /// Finds a specific <see cref="Group"/>.
    /// </summary>
    /// <param name="id">Entity's Primary Key</param>
    Task<Group> FindAsync(string id);

    /// <summary>
    /// Updates a <see cref="Group"/>.
    /// </summary>
    Task UpdateAsync(Group group);

    /// <summary>
    /// Deletes a <see cref="Group"/>.
    /// </summary>
    /// <param name="id">Entity's Primary Key</param>
    Task DeleteAsync(string id);
}
