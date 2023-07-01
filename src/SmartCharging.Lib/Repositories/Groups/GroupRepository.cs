using Microsoft.Extensions.Options;
using SmartCharging.Lib.Models;

namespace SmartCharging.Lib.Repositories.Groups;

public class GroupRepository : Repository<Group>, IGroupRepository
{
    public GroupRepository(IOptions<DatabaseSettings> databaseSettings) : base(databaseSettings.Value, "Groups", "/partitionKey")
    {
    }
}
