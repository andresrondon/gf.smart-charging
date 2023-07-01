using Microsoft.Extensions.Options;
using SmartCharging.Lib.Models;

namespace SmartCharging.Lib.Repositories.ChargeStations;

public class ChargeStationRepository : Repository<ChargeStation>, IChargeStationRepository
{
    public ChargeStationRepository(IOptions<DatabaseSettings> databaseSettings) : base(databaseSettings.Value, "ChargeStations", "/groupId")
    {
    }

    public Task<ICollection<ChargeStation>> FindAllByGroupId(string groupId)
    {
        throw new NotImplementedException();
    }
}
