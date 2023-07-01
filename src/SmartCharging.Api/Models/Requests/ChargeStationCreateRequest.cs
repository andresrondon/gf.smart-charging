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

    public bool Validate(Group? parentGroup, out IEnumerable<string> errors)
    {
        var errorList = new List<string>();

        if (parentGroup is null)
        {
            errorList.Add("Charge Station of ChargeStationId in GroupId does not exists.");
        }

        errors = errorList;

        return !errors.Any();
    }
}
