namespace SmartCharging.Api.Models.Requests;

public class GroupUpdateRequest
{
    public string? Name { get; set; }

    public int? CapacityInAmps { get; set; }
}
