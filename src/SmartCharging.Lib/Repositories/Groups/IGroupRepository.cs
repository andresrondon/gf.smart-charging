using SmartCharging.Lib.Models;

namespace SmartCharging.Lib.Repositories.Groups;

public interface IGroupRepository
{
    Task AddAsync(Group group);
    Task<Group?> FindAsync(string id, string locationArea);
    Task UpdateAsync(Group group);
    Task DeleteAsync(string id, string locationArea);
}
