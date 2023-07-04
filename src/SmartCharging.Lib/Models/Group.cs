using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SmartCharging.Lib.Models;

/// <summary>
/// Represents a group of EV Charging Stations.
/// </summary>
public record Group
{
    [JsonProperty("id")]
    public required string Id { get; set; }

    /// <summary>
    /// General location area of the charging station group. Used as the Partition Key on DB.
    /// </summary>
    [JsonProperty("locationArea")]
    public required string LocationArea { get; set; }

    public required string Name { get; set; }

    /// <summary>
    /// Current Capacity (in Amps). Minimum value of zero.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be minimun {1}")]
    public int CapacityInAmps { get; set; }

    /// <summary>
    /// Charge Stations belonging to the group.
    /// </summary>
    [JsonIgnore]
    public ICollection<ChargeStation> ChargeStations { get; set; } = new List<ChargeStation>();
}
