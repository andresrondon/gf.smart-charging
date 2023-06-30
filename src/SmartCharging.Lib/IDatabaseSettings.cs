namespace SmartCharging.Lib;

public interface IDatabaseSettings
{
    string AccountEndpoint { get; set; }
    string AuthKey { get; set; }
    string DatabaseId { get; set; }
}
