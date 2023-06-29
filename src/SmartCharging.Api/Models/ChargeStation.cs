using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SmartCharging.Api.Models
{
    public class ChargeStation
    {
        [JsonProperty(PropertyName = "id")]
        public required string Id { get; set; }

        public required string Name { get; set; }

        [MaxLength(5)]
        public ICollection<Connector> Connectors { get; set; } = new List<Connector>();
    }
}
