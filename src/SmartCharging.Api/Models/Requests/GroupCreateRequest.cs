using SmartCharging.Lib.Models;

namespace SmartCharging.Api.Models.Requests;

public class GroupCreateRequest
{
    public required string Name { get; set; }

    public int CapacityInAmps { get; set; }

    public Group ToEntity()
    {
        return new Group()
        { 
            Id = Guid.NewGuid().ToString(),
            PartitionKey = "default-partition",
            Name = Name,
            CapacityInAmps = CapacityInAmps
        };
    }
}
