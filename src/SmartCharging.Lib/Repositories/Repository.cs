using Microsoft.Azure.Cosmos;
using SmartCharging.Lib.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace SmartCharging.Lib.Repositories;

public abstract class Repository<TEntity>
{
    protected readonly Database database;
    protected readonly Container container;
    private readonly string partitionKeyPropertyName;

    public Repository(DatabaseSettings databaseSettings, string containerName, string partitionKeyPath)
    {
        CosmosClient client = new(databaseSettings.AccountEndpoint, databaseSettings.AuthKey);
        database = client.GetDatabase(databaseSettings.DatabaseId);
        
        var createContainerIfNotExist = database.CreateContainerIfNotExistsAsync(containerName, partitionKeyPath);
        createContainerIfNotExist.Wait();
        
        partitionKeyPropertyName = partitionKeyPath.Replace("/", "");
        container = createContainerIfNotExist.Result;
    }

    public Task AddAsync(TEntity entity)
    {
        Validator.ValidateObject(entity, new ValidationContext(entity), validateAllProperties: true);
        return container.CreateItemAsync(entity);
    }

    public virtual async Task<TEntity> FindAsync(string id, string partitionKey)
    {
        try
        {
            var response = await container.ReadItemAsync<TEntity>(id, new PartitionKey(partitionKey));
            return response.Resource;
        }
        catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new ResourceNotFoundException($"{typeof(TEntity).Name} not found.")
            {
                Resource = new Dictionary<string, object>()
                {
                    { "id", id },
                    { partitionKeyPropertyName, partitionKey }
                }
            };
        }
    }

    public Task UpdateAsync(TEntity entity)
    {
        Validator.ValidateObject(entity, new ValidationContext(entity), validateAllProperties: true);
        return container.UpsertItemAsync(entity);
    }

    public Task DeleteAsync(string id, string partitionKey)
    {
        return container.DeleteItemAsync<TEntity>(id, new PartitionKey(partitionKey));
    }

    public async Task BulkDeleteAsync(string partitionKey)
    {
        _ = await container.DeleteAllItemsByPartitionKeyStreamAsync(new PartitionKey(partitionKey));
    }
}
