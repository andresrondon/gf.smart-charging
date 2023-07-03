using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Options;
using SmartCharging.Lib.Models;

namespace SmartCharging.Lib.Repositories.Groups;

public class GroupRepository : Repository<Group>, IGroupRepository
{
    protected readonly Container chargeStationContainer;

    public GroupRepository(IOptions<DatabaseSettings> databaseSettings) : base(databaseSettings.Value, "Groups", "/locationArea")
    {
        chargeStationContainer = database.GetContainer(nameof(Group.ChargeStations));
    }

    public override async Task<Group> FindAsync(string id, string locationArea)
    {
        var group = await base.FindAsync(id, locationArea);
        group.ChargeStations = await FindAllChargeStationsByGroupIdAsync(id);
        return group;
    }

    private async Task<ICollection<ChargeStation>> FindAllChargeStationsByGroupIdAsync(string groupId)
    {
        var iterator = chargeStationContainer
            .GetItemLinqQueryable<ChargeStation>()
            .Where(x => x.GroupId == groupId)
            .ToFeedIterator();

        var results = new List<ChargeStation>();
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            results.AddRange(response.ToList());
        }

        return results;
    }
}
