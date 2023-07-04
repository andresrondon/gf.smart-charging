using SmartCharging.Lib.Constants;
using SmartCharging.Lib.Models;

namespace SmartCharging.Api.Requests;

public class GroupCreateRequest
{
    public required string Name { get; set; }

    public int CapacityInAmps { get; set; }

    public Group ToEntity()
    {
        return new Group()
        {
            Id = Guid.NewGuid().ToString(),
            LocationArea = Defaults.Location,
            Name = Name,
            CapacityInAmps = CapacityInAmps
        };
    }
}
