namespace SmartCharging.Lib.Repositories;
using Microsoft.Azure.Cosmos;

public abstract class Repository<TEntity>
{
    private readonly Container container;

    public Repository(DatabaseSettings databaseSettings, string containerName)
    {
        CosmosClient client = new(databaseSettings.AccountEndpoint, databaseSettings.AuthKey);
        Database database = client.GetDatabase(databaseSettings.DatabaseId);
        
        var createContainerIfNotExist = database.CreateContainerIfNotExistsAsync(containerName, "/partitionKey");
        createContainerIfNotExist.Wait();
        
        container = createContainerIfNotExist.Result;
    }

    public Task AddAsync(TEntity entity)
    {
        return container.CreateItemAsync(entity);
    }

    public async Task<TEntity?> FindAsync(string id, string partitionKey)
    {
        try
        {
            var response = await container.ReadItemAsync<TEntity>(id, new PartitionKey(partitionKey));
            return response.Resource;
        }
        catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return default;
        }
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
