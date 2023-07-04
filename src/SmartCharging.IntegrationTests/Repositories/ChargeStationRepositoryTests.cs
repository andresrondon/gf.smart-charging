using Microsoft.Extensions.DependencyInjection;
using SmartCharging.Lib.Repositories.ChargeStations;
using SmartCharging.Lib.Models;
using SmartCharging.Lib.Constants;
using SmartCharging.Lib.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace SmartCharging.IntegrationTests.Repositories;

[Collection(LiveCollection.Name)]
public class ChargeStationRepositoryTests : IAsyncLifetime
{
    private readonly IServiceProvider serviceProvider;
    private readonly IList<ChargeStation> entitiesToDelete;

    public ChargeStationRepositoryTests(LiveFixture fixture)
    {
        serviceProvider = fixture.ServiceProvider;
        entitiesToDelete = new List<ChargeStation>();
    }

    [Fact]
    public async Task ShouldCreateChargeStation()
    {
        // Arrange
        var entity = CreateFakeChargeStation();
        using var scope = serviceProvider.CreateScope();
        var repo = scope.ServiceProvider.GetService<ChargeStationRepository>()!;

        // Act
        var result = await repo.AddAsync(entity);
        entitiesToDelete.Add(entity);

        // Assert
        Assert.Equal(entity.Id, result.Id);
    }

    [Fact]
    public async Task ShouldFindChargeStation()
    {
        // Arrange
        var entity = CreateFakeChargeStation();
        using var scope = serviceProvider.CreateScope();
        var repo = scope.ServiceProvider.GetService<ChargeStationRepository>()!;
        await repo.AddAsync(entity);

        // Act
        var result = await repo.FindAsync(entity.Id, entity.GroupId);
        entitiesToDelete.Add(entity);

        // Assert
        Assert.Equal(entity.Id, result.Id);
    }

    [Fact]
    public async Task ShouldUpdateChargeStation()
    {
        // Arrange
        var entity = CreateFakeChargeStation();
        using var scope = serviceProvider.CreateScope();
        var repo = scope.ServiceProvider.GetService<ChargeStationRepository>()!;
        await repo.AddAsync(entity);
        entity.Name = "New Station";

        // Act
        var result = await repo.UpdateAsync(entity);
        entitiesToDelete.Add(entity);

        // Assert
        Assert.Equal(entity.Name, result.Name);
    }

    [Fact]
    public async Task ShouldDeleteChargeStation()
    {
        // Arrange
        var entity = CreateFakeChargeStation();
        using var scope = serviceProvider.CreateScope();
        var repo = scope.ServiceProvider.GetService<ChargeStationRepository>()!;
        var result = await repo.AddAsync(entity);

        // Act
        await repo.DeleteAsync(entity.Id, entity.GroupId);

        // Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => repo.FindAsync(result.Id, result.GroupId));
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionIfAddingChargeStationWithNoConnectors()
    {
        // Arrange
        var entity = CreateFakeChargeStation();
        entity.Connectors = new List<Connector>();
        using var scope = serviceProvider.CreateScope();
        var repo = scope.ServiceProvider.GetService<ChargeStationRepository>()!;

        // Act
        var task = repo.AddAsync(entity);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(() => task);
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionIfUpdatingChargeStationWithNoConnectors()
    {
        // Arrange
        var entity = CreateFakeChargeStation();
        using var scope = serviceProvider.CreateScope();
        var repo = scope.ServiceProvider.GetService<ChargeStationRepository>()!;
        await repo.AddAsync(entity);
        entitiesToDelete.Add(entity);
        entity.Connectors = new List<Connector>();

        // Act
        var task = repo.UpdateAsync(entity);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(() => task);
    }

    private static ChargeStation CreateFakeChargeStation()
    {
        return new ChargeStation
        {
            Id = Guid.NewGuid().ToString(),
            GroupId = Guid.NewGuid().ToString(),
            Name = "ChargeStation 1",
            Connectors =
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 }
            }
        };
    }

    public async Task DisposeAsync()
    {
        using var scope = serviceProvider.CreateScope();
        var repo = scope.ServiceProvider.GetService<ChargeStationRepository>()!;
        foreach (var item in entitiesToDelete)
        {
            await repo.DeleteAsync(item.Id, item.GroupId);
        }
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }
}
