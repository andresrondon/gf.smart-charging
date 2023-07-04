namespace SmartCharging.Lib;

/// <summary>
/// Basic settings required to stablish a connection with a Azure Cosmos DB instance.
/// </summary>
public class DatabaseSettings
{
    /// <summary>
    /// Cosmos DB service URI.
    /// </summary>
    public required string AccountEndpoint { get; set; }

    /// <summary>
    /// Auth key for authenticating to Cosmos DB instance.
    /// </summary>
    public required string AuthKey { get; set; }

    /// <summary>
    /// Name of the Database in Cosmos DB.
    /// </summary>
    public required string DatabaseId { get; set; }
}
