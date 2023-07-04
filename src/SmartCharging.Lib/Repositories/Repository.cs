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
        var createDatabaseIfNotExist = client.CreateDatabaseIfNotExistsAsync(databaseSettings.DatabaseId);
        createDatabaseIfNotExist.Wait();
        database = createDatabaseIfNotExist.Result;

        var createContainerIfNotExist = database.CreateContainerIfNotExistsAsync(containerName, partitionKeyPath);
        createContainerIfNotExist.Wait();
        container = createContainerIfNotExist.Result;
        
        partitionKeyPropertyName = partitionKeyPath.Replace("/", "");
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        Validator.ValidateObject(entity, new ValidationContext(entity), validateAllProperties: true);
        var response = await container.CreateItemAsync(entity);
        return response.Resource;
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

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        Validator.ValidateObject(entity, new ValidationContext(entity), validateAllProperties: true);
        var response = await container.UpsertItemAsync(entity);
        return response.Resource;
    }

    public Task DeleteAsync(string id, string partitionKey)
    {
        return container.DeleteItemAsync<TEntity>(id, new PartitionKey(partitionKey));
    }
}
