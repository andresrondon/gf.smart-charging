using SmartCharging.Lib.Models;

namespace SmartCharging.Api.Requests;

public class ConnectorCreateRequest
{
    public int Id { get; set; }

    public int MaxCurrentInAmps { get; set; }

    public Connector ToEntity()
    {
        return new Connector
        {
            Id = Id,
            MaxCurrentInAmps = MaxCurrentInAmps
        };
    }
}
