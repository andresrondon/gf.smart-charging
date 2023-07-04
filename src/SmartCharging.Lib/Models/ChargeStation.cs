using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SmartCharging.Lib.Models;

/// <summary>
/// Represents a EV charge station.
/// </summary>
public record ChargeStation
{
    [JsonProperty("id")]
    public required string Id { get; set; }

    [JsonProperty("groupId")]
    public required string GroupId { get; set; }

    public required string Name { get; set; }

    /// <summary>
    /// List of connectors that correspond to this station. Must have at least 1.
    /// </summary>
    [MinLength(1, ErrorMessage = "A station must have at least {1} connector."), MaxLength(5)]
    public ICollection<Connector> Connectors { get; set; } = new List<Connector>();

    /// <summary>
    /// Sum of its connectors' Max Current (in Amps).
    /// </summary>
    [JsonIgnore]
    public int MaxCurrentInAmpsSum => Connectors.Sum(c => c.MaxCurrentInAmps);
}
