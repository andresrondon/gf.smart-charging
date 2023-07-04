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

    public async Task BulkDeleteAsync(string groupId)
    {
        // vvvvv Cosmos DB SDK Preview Version only vvvvv
        /* 
         * await container.DeleteAllItemsByPartitionKeyStreamAsync(new PartitionKey(groupId)); 
         */

        // vvvvv Workaround vvvvv
        var iterator = container
            .GetItemLinqQueryable<ChargeStation>()
            .Where(x => x.GroupId == groupId)
            .ToFeedIterator();

        while (iterator.HasMoreResults)
        {
            var response = (await iterator.ReadNextAsync()).ToList();
            Task.WaitAll(response.Select(x => DeleteAsync(x.Id, x.GroupId)).ToArray());
        }
    }
}
