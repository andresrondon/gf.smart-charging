using SmartCharging.Lib.Models;

namespace SmartCharging.Api.Models.Requests;

public class ConnectorCreateRequest
{
    public int Id { get; set; }

    public required string ChargeStationId { get; set; }

    public required string GroupId { get; set; }

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

    public bool Validate(Group? parentGroup, ChargeStation? parentStation, out IEnumerable<string> errors)
    {
        var errorList = new List<string>();

        if (parentGroup is null)
        {
            errorList.Add("Group of GroupId does not exists.");
        }
        else if (parentStation is null)
        {
            errorList.Add("Charge Station of ChargeStationId in GroupId does not exists.");
        }

        if (!(Id >= 1 && Id <= 5))
        {
            errorList.Add("Id must be between 1 and 5.");
        }

        if (parentGroup is not null && parentGroup.MaxCurrentInAmpsSum + MaxCurrentInAmps > parentGroup.CapacityInAmps)
        {
            errorList.Add($"Adding this connector's Max Current exceeds the Group's Capacity. Group's Capacity in Amps: {parentGroup.CapacityInAmps}. Sum of all its connectors' Max Current: {parentGroup.MaxCurrentInAmpsSum}.");
        }

        errors = errorList;

        return !errors.Any();
    }
}
