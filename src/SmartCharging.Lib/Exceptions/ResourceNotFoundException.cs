namespace SmartCharging.Lib.Exceptions;

public class ResourceNotFoundException : Exception
{
    public Dictionary<string, object>? Resource { get; init; }
    
    public ResourceNotFoundException(string? message) : base(message)
    {
    }
}
