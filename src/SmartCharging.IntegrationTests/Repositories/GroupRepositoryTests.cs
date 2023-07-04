using Microsoft.Extensions.DependencyInjection;
using SmartCharging.Lib.Repositories.Groups;
using SmartCharging.Lib.Models;
using SmartCharging.Lib.Constants;
using SmartCharging.Lib.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace SmartCharging.IntegrationTests.Repositories;

[Collection(LiveCollection.Name)]
public class GroupRepositoryTests : IAsyncLifetime
{
    private readonly IServiceProvider serviceProvider;
    private readonly IList<Group> entitiesToDelete;

    public GroupRepositoryTests(LiveFixture fixture)
    {
        serviceProvider = fixture.ServiceProvider;
        entitiesToDelete = new List<Group>();
    }

    [Fact]
    public async Task ShouldCreateGroup()
    {
        // Arrange
        var entity = CreateFakeGroup();
        using var scope = serviceProvider.CreateScope();
        var repo = scope.ServiceProvider.GetService<GroupRepository>()!;

        // Act
        var result = await repo.AddAsync(entity);
        entitiesToDelete.Add(entity);

        // Assert
        Assert.Equal(entity.Id, result.Id);
    }

    [Fact]
    public async Task ShouldFindGroup()
    {
        // Arrange
        var entity = CreateFakeGroup();
        using var scope = serviceProvider.CreateScope();
        var repo = scope.ServiceProvider.GetService<GroupRepository>()!;
        await repo.AddAsync(entity);

        // Act
        var result = await repo.FindAsync(entity.Id, entity.LocationArea);
        entitiesToDelete.Add(entity);

        // Assert
        Assert.Equal(entity.Id, result.Id);
    }

    [Fact]
    public async Task ShouldUpdateGroup()
    {
        // Arrange
        var entity = CreateFakeGroup();
        using var scope = serviceProvider.CreateScope();
        var repo = scope.ServiceProvider.GetService<GroupRepository>()!;
        await repo.AddAsync(entity);
        entity.CapacityInAmps = 1;

        // Act
        var result = await repo.UpdateAsync(entity);
        entitiesToDelete.Add(entity);

        // Assert
        Assert.Equal(entity.CapacityInAmps, result.CapacityInAmps);
    }

    [Fact]
    public async Task ShouldDeleteGroup()
    {
        // Arrange
        var entity = CreateFakeGroup();
        using var scope = serviceProvider.CreateScope();
        var repo = scope.ServiceProvider.GetService<GroupRepository>()!;
        var result = await repo.AddAsync(entity);

        // Act
        await repo.DeleteAsync(entity.Id, entity.LocationArea);

        // Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => repo.FindAsync(result.Id, result.LocationArea));
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionIfAddingGroupWithZeroOrLessCapacity()
    {
        // Arrange
        var entity = CreateFakeGroup(capacityInAmps: 0);
        using var scope = serviceProvider.CreateScope();
        var repo = scope.ServiceProvider.GetService<GroupRepository>()!;

        // Act
        var task = repo.AddAsync(entity);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(() => task);
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionIfUpdatingGroupWithZeroOrLessCapacity()
    {
        // Arrange
        var entity = CreateFakeGroup();
        using var scope = serviceProvider.CreateScope();
        var repo = scope.ServiceProvider.GetService<GroupRepository>()!;
        await repo.AddAsync(entity);
        entitiesToDelete.Add(entity);
        entity.CapacityInAmps = 0;

        // Act
        var task = repo.UpdateAsync(entity);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(() => task);
    }

    private static Group CreateFakeGroup(int capacityInAmps = 10)
    {
        return new Group
        {
            Id = Guid.NewGuid().ToString(),
            LocationArea = Defaults.Location,
            Name = "Group 1",
            CapacityInAmps = capacityInAmps
        };
    }

    public async Task DisposeAsync()
    {
        using var scope = serviceProvider.CreateScope();
        var repo = scope.ServiceProvider.GetService<GroupRepository>()!;
        foreach (var item in entitiesToDelete)
        {
            await repo.DeleteAsync(item.Id, item.LocationArea);
        }
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }
}
