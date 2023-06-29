using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SmartCharging.Api.Models;

public class Connector
{
    [JsonProperty(PropertyName = "id"), Range(1, 5)]
    public int Id { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be minimun {1}")]
    public int MaxCurrentInAmps { get; set; }
}
