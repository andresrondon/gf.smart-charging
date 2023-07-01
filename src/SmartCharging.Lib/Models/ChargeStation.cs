﻿using System.ComponentModel.DataAnnotations;

namespace SmartCharging.Lib.Models;

public record ChargeStation
{
    public required string Id { get; set; }

    public required string GroupId { get; set; }

    public required string Name { get; set; }

    [MaxLength(5, ErrorMessage = "A Charging Station cannot have more than {1} {0}")]
    public ICollection<Connector> Connectors { get; set; } = new List<Connector>();
}
