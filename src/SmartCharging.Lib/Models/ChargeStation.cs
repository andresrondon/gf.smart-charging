using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SmartCharging.Lib.Models;

public record ChargeStation
{
    [JsonProperty("id")]
    public required string Id { get; set; }

    [JsonProperty("groupId")]
    public required string GroupId { get; set; }

    public required string Name { get; set; }

    [MinLength(1, ErrorMessage = "A Charging Station must have at least {1} {0}")]
    [MaxLength(5, ErrorMessage = "A Charging Station cannot have more than {1} {0}")]
    public ICollection<Connector> Connectors { get; set; } = new List<Connector>();
}
