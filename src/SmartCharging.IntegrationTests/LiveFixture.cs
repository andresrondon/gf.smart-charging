using Microsoft.Extensions.DependencyInjection;
using SmartCharging.Lib.Repositories.Groups;
using SmartCharging.Lib;
using SmartCharging.Lib.Repositories.ChargeStations;

namespace SmartCharging.IntegrationTests;

/// <summary>
///     Fixture that provides access to live services.
/// </summary>
public class LiveFixture
{
    public IServiceProvider ServiceProvider { get; }

    public LiveFixture() 
    {
        IServiceCollection serviceCollection = new ServiceCollection();

        serviceCollection.Configure<DatabaseSettings>(x =>
        {
            x.AccountEndpoint = "https://localhost:8081";
            x.AuthKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
            x.DatabaseId = "SmartChargingDB";
        });

        // Repositories
        serviceCollection.AddScoped<GroupRepository>();
        serviceCollection.AddScoped<ChargeStationRepository>();

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }
}
