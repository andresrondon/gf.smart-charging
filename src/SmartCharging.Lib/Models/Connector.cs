using System.ComponentModel.DataAnnotations;

namespace SmartCharging.Lib.Models;

/// <summary>
/// Represents a EV Charge Station's connector.
/// </summary>
public class Connector
{
    [Range(1, 5)]
    public required int Id { get; set; }

    /// <summary>
    /// Max Current (in Amps). Minimum value of 1.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be minimun {1}")]
    public required int MaxCurrentInAmps { get; set; }
}
