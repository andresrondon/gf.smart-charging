namespace SmartCharging.Lib.Repositories;
using Microsoft.Azure.Cosmos;

public abstract class Repository<TEntity>
{
    private readonly Container container;

    public Repository(IDatabaseSettings databaseSettings, string containerName)
    {
        using CosmosClient client = new(databaseSettings.AccountEndpoint, databaseSettings.AuthKey);
        
        container = client
            .GetDatabase(databaseSettings.DatabaseId)
            .GetContainer(containerName);
    }

    public Task AddAsync(TEntity entity)
    {
        return container.CreateItemAsync(entity);
    }

    public async Task<TEntity> FindAsync(string id, string partitionKey)
    {
        var response = await container.ReadItemAsync<TEntity>(id, new PartitionKey(partitionKey));
        return response.Resource;
    }

    public Task UpdateAsync(TEntity entity)
    {
        return container.UpsertItemAsync(entity);
    }

    public Task DeleteAsync(string id, string partitionKey) 
    {
        return container.DeleteItemAsync<TEntity>(id, new PartitionKey(partitionKey));
    }
}
