using Moq;
using SmartCharging.Lib.Constants;
using SmartCharging.Lib.Exceptions;
using SmartCharging.Lib.Models;
using SmartCharging.Lib.Repositories.ChargeStations;
using SmartCharging.Lib.Repositories.Groups;
using SmartCharging.Lib.Services.ChargeStations;
using SmartCharging.Lib.Services.Groups;
using System.ComponentModel.DataAnnotations;

namespace SmartCharging.Tests.Services;

public class GroupServiceTests
{
    private Mock<IChargeStationRepository> _chargeStationRepositoryMock;
    private Mock<IGroupRepository> _groupRepositoryMock;

    [Fact]
    public async Task ShouldAddNewGroup()
    {
        var parentGroup = new Group
        {
            Id = Guid.NewGuid().ToString(),
            LocationArea = Defaults.Location,
            Name = "Group 1",
            CapacityInAmps = 10
        };

        var service = BuildService();
        await service.AddAsync(parentGroup);

        _groupRepositoryMock.Verify(m => m.AddAsync(parentGroup), Times.Once());
    }

    [Fact]
    public async Task ShouldDeleteGroupIncludingChildStations()
    {
        var groupId = Guid.NewGuid().ToString();

        var service = BuildService();
        await service.DeleteAsync(groupId);

        _groupRepositoryMock.Verify(m => m.DeleteAsync(groupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.BulkDeleteAsync(groupId), Times.Once());
    }

    [Fact]
    public async Task ShouldFindGroup()
    {
        var group = new Group
        {
            Id = Guid.NewGuid().ToString(),
            LocationArea = Defaults.Location,
            Name = "Group 1",
            CapacityInAmps = 10
        };

        var service = BuildService(group);
        await service.FindAsync(group.Id);

        _groupRepositoryMock.Verify(m => m.FindAsync(group.Id, Defaults.Location), Times.Once());
    }

    [Fact]
    public async Task ShouldUpdateGroup()
    {
        var group = new Group
        {
            Id = Guid.NewGuid().ToString(),
            LocationArea = Defaults.Location,
            Name = "Group 1",
            CapacityInAmps = 10
        };

        var service = BuildService();
        await service.UpdateAsync(group);

        _groupRepositoryMock.Verify(m => m.UpdateAsync(group), Times.Once());
    }

    [Fact]
    public async Task ShouldNotAddNewGroupIfCapacityIsNotAPositiveInteger()
    {
        var group = new Group
        {
            Id = Guid.NewGuid().ToString(),
            LocationArea = Defaults.Location,
            Name = "Group 1",
            CapacityInAmps = 0
        };

        var service = BuildService();
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.AddAsync(group));

        _groupRepositoryMock.Verify(m => m.AddAsync(group), Times.Never());
    }

    [Fact]
    public async Task ShouldNotUpdateGroupIfCapacityIsNotAPositiveInteger()
    {
        var group = new Group
        {
            Id = Guid.NewGuid().ToString(),
            LocationArea = Defaults.Location,
            Name = "Group 1",
            CapacityInAmps = 0
        };

        var service = BuildService();
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.UpdateAsync(group));

        _groupRepositoryMock.Verify(m => m.UpdateAsync(group), Times.Never());
    }

    [Fact]
    public async Task ShouldNotUpdateGroupIfCapacityIsLowerThanTheSumOfItsConnectorsMaxCurrent()
    {
        string groupId = Guid.NewGuid().ToString();
        var parentGroup = new Group
        {
            Id = groupId,
            LocationArea = Defaults.Location,
            Name = "Group 1",
            CapacityInAmps = 10,
            ChargeStations =
            {
                new ChargeStation
                {
                    GroupId = groupId,
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
                    GroupId = groupId,
                    Name = "Station 2",
                    Connectors =
                    {
                        new Connector { Id = 1, MaxCurrentInAmps = 1 },
                        new Connector { Id = 2, MaxCurrentInAmps = 2 }
                    }
                }
            }
        };

        var service = BuildService();
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.UpdateAsync(parentGroup));

        _groupRepositoryMock.Verify(m => m.UpdateAsync(parentGroup), Times.Never());
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
            .Returns(Task.CompletedTask);
        _groupRepositoryMock
            .Setup(m => m.DeleteAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        _groupRepositoryMock
            .Setup(m => m.UpdateAsync(It.IsAny<Group>()))
            .Returns(Task.CompletedTask);
        
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
}
