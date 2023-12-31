﻿using SmartCharging.Lib.Models;

namespace SmartCharging.Api.Requests;

public class ChargeStationUpdateRequest
{
    public string? Name { get; set; }

    public ICollection<Connector>? Connectors { get; set; }
}
