using SmartCharging.Lib.Models;

namespace SmartCharging.Lib.Services.Groups;

public interface IGroupService
{
    Task AddAsync(Group group);
    Task<Group> FindAsync(string location, string groupId);
    Task UpdateAsync(Group group);
    Task DeleteAsync(string location, string id);
}
