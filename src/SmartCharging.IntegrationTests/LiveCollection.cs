namespace SmartCharging.IntegrationTests;

[CollectionDefinition(Name)]
public class LiveCollection : ICollectionFixture<LiveFixture>
{
    public const string Name = nameof(LiveCollection);
}
