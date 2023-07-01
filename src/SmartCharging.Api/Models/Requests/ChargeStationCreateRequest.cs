using SmartCharging.Lib.Models;

namespace SmartCharging.Api.Models.Requests;

public class ChargeStationCreateRequest
{
    public required string GroupId { get; set; }

    public required string Name { get; set; }

    public ChargeStation ToEntity()
    {
        return new ChargeStation
        { 
            Id = Guid.NewGuid().ToString(), 
            GroupId = GroupId, 
            Name = Name 
        };
    }
}
