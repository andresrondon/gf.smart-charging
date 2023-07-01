using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SmartCharging.Lib.Models;

public record Connector
{
    [JsonProperty(PropertyName = "id")]
    public required string EntityId { get; set; }

    public required string ChargeStationId { get; set; }

    [Range(1, 5)]
    [JsonProperty(PropertyName = "numberInStation")]
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be minimun {1}")]
    public int MaxCurrentInAmps { get; set; }
}
