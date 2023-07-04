using Moq;
using SmartCharging.Lib.Constants;
using SmartCharging.Lib.Exceptions;
using SmartCharging.Lib.Models;
using SmartCharging.Lib.Repositories.ChargeStations;
using SmartCharging.Lib.Repositories.Groups;
using SmartCharging.Lib.Services.Groups;

namespace SmartCharging.Tests.Services;

public class GroupServiceTests
{
    private Mock<IChargeStationRepository> _chargeStationRepositoryMock = new();
    private Mock<IGroupRepository> _groupRepositoryMock = new();

    [Fact]
    public async Task ShouldAddNewGroup()
    {
        // Arrange
        var group = CreateGroupFake();
        var service = BuildService();

        // Act
        await service.AddAsync(group);

        // Assert
        _groupRepositoryMock.Verify(m => m.AddAsync(group), Times.Once());
    }

    [Fact]
    public async Task ShouldDeleteGroupIncludingChildStations()
    {
        // Arrange
        var groupId = Guid.NewGuid().ToString();
        var service = BuildService();

        // Act
        await service.DeleteAsync(groupId);

        // Assert
        _groupRepositoryMock.Verify(m => m.DeleteAsync(groupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.BulkDeleteAsync(groupId), Times.Once());
    }

    [Fact]
    public async Task ShouldFindGroup()
    {
        // Arrange
        var group = CreateGroupFake();
        var service = BuildService(group);

        // Act
        await service.FindAsync(group.Id);

        _groupRepositoryMock.Verify(m => m.FindAsync(group.Id, Defaults.Location), Times.Once());
    }

    [Fact]
    public async Task ShouldUpdateGroup()
    {
        // Arrange
        var group = CreateGroupFake();
        var service = BuildService();

        // Act
        await service.UpdateAsync(group);

        _groupRepositoryMock.Verify(m => m.UpdateAsync(group), Times.Once());
    }

    [Fact]
    public async Task ShouldNotAddNewGroupIfCapacityIsNotAPositiveInteger()
    {
        // Arrange
        var group = CreateGroupFake(capacityInAmps: 0);
        var service = BuildService();

        // Act / Assert
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.AddAsync(group));

        _groupRepositoryMock.Verify(m => m.AddAsync(group), Times.Never());
    }

    [Fact]
    public async Task ShouldNotUpdateGroupIfCapacityIsNotAPositiveInteger()
    {
        // Arrange
        var group = CreateGroupFake(capacityInAmps: 0);
        var service = BuildService();

        // Act / Assert
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.UpdateAsync(group));

        _groupRepositoryMock.Verify(m => m.UpdateAsync(group), Times.Never());
    }

    [Fact]
    public async Task ShouldNotUpdateGroupIfCapacityIsLowerThanTheSumOfItsConnectorsMaxCurrent()
    {
        // Arrange
        var group = CreateGroupFake(capacityInAmps: 0);
        group.ChargeStations = new List<ChargeStation>
        {
            new ChargeStation
            {
                GroupId = group.Id,
                Id = Guid.NewGuid().ToString(),
                Name = "Station 1",
                Connectors =
                {
                    new Connector { Id = 1, MaxCurrentInAmps = 8 }
                }
            },
            new ChargeStation
            {
                Id = Guid.NewGuid().ToString(),
                GroupId = group.Id,
                Name = "Station 2",
                Connectors =
                {
                    new Connector { Id = 1, MaxCurrentInAmps = 1 },
                    new Connector { Id = 2, MaxCurrentInAmps = 2 }
                }
            }
        };
        var service = BuildService();

        // Act / Assert
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.UpdateAsync(group));

        _groupRepositoryMock.Verify(m => m.UpdateAsync(group), Times.Never());
    }

    private GroupService BuildService(Group? expectedResult = null)
    {
        _chargeStationRepositoryMock = new Mock<IChargeStationRepository>();
        _chargeStationRepositoryMock
            .Setup(m => m.BulkDeleteAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _groupRepositoryMock = new Mock<IGroupRepository>();
        _groupRepositoryMock
            .Setup(m => m.AddAsync(It.IsAny<Group>()))
            .Returns(Task.FromResult(It.IsAny<Group>()));
        _groupRepositoryMock
            .Setup(m => m.DeleteAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        _groupRepositoryMock
            .Setup(m => m.UpdateAsync(It.IsAny<Group>()))
            .Returns(Task.FromResult(It.IsAny<Group>()));
        
        if (expectedResult is not null)
        {
            _groupRepositoryMock
                .Setup(m => m.FindAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(expectedResult));
        }
        else
        {
            _groupRepositoryMock
                .Setup(m => m.FindAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new ResourceNotFoundException("Not found."));
        }

        return new GroupService(_groupRepositoryMock.Object, _chargeStationRepositoryMock.Object);
    }

    private static Group CreateGroupFake(int capacityInAmps = 10)
    {
        return new Group
        {
            Id = Guid.NewGuid().ToString(),
            LocationArea = Defaults.Location,
            Name = "Group 1",
            CapacityInAmps = capacityInAmps
        };
    }
}
