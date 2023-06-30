using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SmartCharging.Lib.Models;

public record Group
{
    [JsonProperty(PropertyName = "id")]
    public required string Id { get; set; }

    public required string Name { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be minimun {1}")]
    public int CapacityInAmps { get; set; }

    public ICollection<ChargeStation> ChargeStations { get; set; } = new List<ChargeStation>();

    /// <summary>
    ///     Sum of the Max Current (in amps) of all Connectors indirectly belonging to the group.
    /// </summary>
    public int MaxCurrentInAmpsSum => ChargeStations.Sum(cs => cs.Connectors.Sum(c => c.MaxCurrentInAmps));
}
