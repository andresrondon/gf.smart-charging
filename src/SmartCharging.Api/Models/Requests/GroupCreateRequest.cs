using SmartCharging.Lib.Models;

namespace SmartCharging.Api.Models.Requests;

public class GroupCreateRequest
{
    public required string Name { get; set; }

    public int CapacityInAmps { get; set; }

    public Group ToEntity(string locationArea)
    {
        return new Group()
        { 
            Id = Guid.NewGuid().ToString(),
            LocationArea = locationArea,
            Name = Name,
            CapacityInAmps = CapacityInAmps
        };
    }
}
