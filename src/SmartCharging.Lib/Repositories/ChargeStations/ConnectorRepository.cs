using Microsoft.Extensions.Options;
using SmartCharging.Lib.Models;

namespace SmartCharging.Lib.Repositories.ChargeStations;

public class ChargeStationRepository : Repository<ChargeStation>, IChargeStationRepository
{
    public ChargeStationRepository(IOptions<IDatabaseSettings> databaseSettings) : base(databaseSettings.Value, "ChargeStations")
    {
    }
}
