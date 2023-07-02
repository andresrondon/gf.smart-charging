using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SmartCharging.Lib.Models;

public class Connector
{
    [Range(1, 5)]
    public required int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be minimun {1}")]
    public required int MaxCurrentInAmps { get; set; }
}
