using SmartCharging.Lib.Models;

namespace SmartCharging.Lib.Repositories.Groups;

public interface IGroupRepository
{
    Task<Group> AddAsync(Group group);
    Task<Group> FindAsync(string id, string locationArea);
    Task<Group> UpdateAsync(Group group);
    Task DeleteAsync(string id, string locationArea);
}
