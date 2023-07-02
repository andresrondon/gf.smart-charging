using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Options;
using SmartCharging.Lib.Models;

namespace SmartCharging.Lib.Repositories.ChargeStations;

public class ChargeStationRepository : Repository<ChargeStation>, IChargeStationRepository
{
    public ChargeStationRepository(IOptions<DatabaseSettings> databaseSettings) 
        : base(databaseSettings.Value, "ChargeStations", "/groupId")
    {
    }

    public async Task<ICollection<ChargeStation>> FindAllByGroupIdAsync(string groupId)
    {
        var iterator = container
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
