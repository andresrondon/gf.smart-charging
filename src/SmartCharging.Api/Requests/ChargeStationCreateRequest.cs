using SmartCharging.Lib.Models;

namespace SmartCharging.Api.Requests;

public class ChargeStationCreateRequest
{
    public required string Name { get; set; }

    public required ICollection<Connector> Connectors { get; set; }

    public ChargeStation ToEntity(string groupId)
    {
        return new ChargeStation
        {
            Id = Guid.NewGuid().ToString(),
            GroupId = groupId,
            Name = Name,
            Connectors = Connectors
        };
    }
}
