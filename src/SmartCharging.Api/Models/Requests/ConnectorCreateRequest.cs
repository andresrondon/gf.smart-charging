using SmartCharging.Lib.Models;

namespace SmartCharging.Api.Models.Requests;

public class ConnectorCreateRequest
{
    public int Id { get; set; }

    public required string ChargeStationId { get; set; }

    public int MaxCurrentInAmps { get; set; }

    public Connector ToEntity()
    {
        return new Connector
        {
            EntityId = Guid.NewGuid().ToString(),
            Id = Id,
            ChargeStationId = ChargeStationId,
            MaxCurrentInAmps = MaxCurrentInAmps
        };
    }
}
