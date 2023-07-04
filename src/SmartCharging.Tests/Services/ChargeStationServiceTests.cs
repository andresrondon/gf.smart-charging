using Moq;
using SmartCharging.Lib.Constants;
using SmartCharging.Lib.Exceptions;
using SmartCharging.Lib.Models;
using SmartCharging.Lib.Repositories.ChargeStations;
using SmartCharging.Lib.Repositories.Groups;
using SmartCharging.Lib.Services.ChargeStations;

namespace SmartCharging.Tests.Services;

public class ChargeStationServiceTests
{
    private Mock<IChargeStationRepository> _chargeStationRepositoryMock;
    private Mock<IGroupRepository> _groupRepositoryMock;

    [Fact]
    public async Task ShouldAddNewChargeStation()
    {
        // Arrange
        var parentGroup = CreateGroupFake(capacityInAmps: 10);
        var station = CreateChargeStationFake(parentGroup.Id);
        var service = BuildService(parentGroup);

        // Act
        await service.AddAsync(station);

        // Assert
        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.AddAsync(station), Times.Once());
    }

    [Fact]
    public async Task ShouldDeleteChargeStation()
    {
        // Arrange
        var stationId = Guid.NewGuid().ToString();
        var groupId = Guid.NewGuid().ToString();
        var service = BuildService();

        // Act
        await service.DeleteAsync(groupId, stationId);

        // Assert
        _chargeStationRepositoryMock.Verify(m => m.DeleteAsync(stationId, groupId), Times.Once());
    }

    [Fact]
    public async Task ShouldFindChargeStation()
    {
        // Arrange
        var station = CreateChargeStationFake();
        var service = BuildService(null, station);

        // Act
        await service.FindAsync(station.GroupId, station.Id);

        // Assert
        _chargeStationRepositoryMock.Verify(m => m.FindAsync(station.Id, station.GroupId), Times.Once());
    }

    [Fact]
    public async Task ShouldUpdateChargeStation()
    {
        // Arrange
        var parentGroup = CreateGroupFake(capacityInAmps: 10);
        var station = CreateChargeStationFake(parentGroup.Id);
        var service = BuildService(parentGroup);

        // Act
        await service.UpdateAsync(station);

        // Assert
        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(station), Times.Once());
    }

    [Fact]
    public async Task ShouldNotAddNewChargeStationWithNoConnectors()
    {
        // Arrange
        var parentGroup = CreateGroupFake(capacityInAmps: 10);
        var station = CreateChargeStationFake(parentGroup.Id, connectors: new List<Connector>());
        var service = BuildService(parentGroup);

        // Act / Assert
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.AddAsync(station));

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.AddAsync(station), Times.Never());
    }

    [Fact]
    public async Task ShouldNotAddNewChargeStationWithMoreThanFiveConnectors()
    {
        // Arrange
        var parentGroup = CreateGroupFake(capacityInAmps: 10);
        var station = CreateChargeStationFake(
            parentGroup.Id, 
            connectors: new List<Connector>
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 1 },
                new Connector { Id = 3, MaxCurrentInAmps = 1 },
                new Connector { Id = 4, MaxCurrentInAmps = 1 },
                new Connector { Id = 5, MaxCurrentInAmps = 1 },
                new Connector { Id = 6, MaxCurrentInAmps = 1 }
            });

        var service = BuildService(parentGroup);
        
        // Act / Assert
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.AddAsync(station));

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.AddAsync(station), Times.Never());
    }

    [Fact]
    public async Task ShouldNotUpdateChargeStationWithNoConnectors()
    {
        // Arrange
        var parentGroup = CreateGroupFake(capacityInAmps: 10);
        var station = CreateChargeStationFake(parentGroup.Id, connectors: new List<Connector>());
        var service = BuildService(parentGroup);

        // Act / Assert
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.UpdateAsync(station));

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(station), Times.Never());
    }

    [Fact]
    public async Task ShouldNotUpdateChargeStationWithMoreThanFiveConnectors()
    {
        // Arrange
        var parentGroup = CreateGroupFake(capacityInAmps: 10);
        var station = CreateChargeStationFake(
            parentGroup.Id,
            connectors: new List<Connector>
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 1 },
                new Connector { Id = 3, MaxCurrentInAmps = 1 },
                new Connector { Id = 4, MaxCurrentInAmps = 1 },
                new Connector { Id = 5, MaxCurrentInAmps = 1 },
                new Connector { Id = 6, MaxCurrentInAmps = 1 }
            });

        var service = BuildService(parentGroup);
        
        // Act / Assert
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.UpdateAsync(station));

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(station), Times.Never());
    }

    [Fact]
    public async Task ShouldNotAddNewChargeStationIfGroupDoesNotExists()
    {
        // Arrange
        var station = CreateChargeStationFake(parentGroupId: "incorrect group id");
        var service = BuildService(parentGroup: null);

        // Act / Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => service.AddAsync(station));

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.AddAsync(station), Times.Never());
    }

    [Fact]
    public async Task ShouldNotUpdateChargeStationIfGroupDoesNotExists()
    {
        // Arrange
        var station = CreateChargeStationFake(parentGroupId: "incorrect group id");
        var service = BuildService(parentGroup: null);

        // Act / Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => service.UpdateAsync(station));

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(station), Times.Never());
    }

    [Fact]
    public async Task ShouldNotAddNewChargeStationIfConnectorsHaveDuplicateIds()
    {
        // Arrange
        var parentGroup = CreateGroupFake(capacityInAmps: 10);
        var station = CreateChargeStationFake(
            parentGroup.Id,
            connectors: new List<Connector>
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 1 }
            });

        var service = BuildService(parentGroup);
        
        // Act / Assert
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.AddAsync(station));

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.AddAsync(station), Times.Never());
    }

    [Fact]
    public async Task ShouldNotUpdateChargeStationIfConnectorsHaveDuplicateIds()
    {
        // Arrange
        var parentGroup = CreateGroupFake(capacityInAmps: 10);
        var station = CreateChargeStationFake(
            parentGroup.Id,
            connectors: new List<Connector>
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 1 }
            });

        var service = BuildService(parentGroup);
        
        // Act / Assert
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.UpdateAsync(station));

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(station), Times.Never());
    }

    [Fact]
    public async Task ShouldNotAddNewChargeStationIfConnectorsDoNotHavePositiveMaxCurrent()
    {
        // Arrange
        var parentGroup = CreateGroupFake(capacityInAmps: 10);
        var station = CreateChargeStationFake(
            parentGroup.Id,
            connectors: new List<Connector>
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 0 },
            });

        var service = BuildService(parentGroup);
        
        // Act / Assert
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.AddAsync(station));

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.AddAsync(station), Times.Never());
    }

    [Fact]
    public async Task ShouldNotUpdateChargeStationIfConnectorsDoNotHavePositiveMaxCurrent()
    {
        // Arrange
        var parentGroup = CreateGroupFake(capacityInAmps: 10);
        var station = CreateChargeStationFake(
            parentGroup.Id,
            connectors: new List<Connector>
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 0 },
            });

        var service = BuildService(parentGroup);
        
        // Act / Assert
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.UpdateAsync(station));

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(station), Times.Never());
    }

    [Fact]
    public async Task ShouldNotAddNewChargeStationIfTheSumOfItsConnectorsMaxCurrentExceedsGroupsCapacity()
    {
        // Arrange
        var parentGroup = CreateGroupFake(capacityInAmps: 10);
        parentGroup.ChargeStations.Add(
            CreateChargeStationFake(
                parentGroup.Id,
                connectors: new List<Connector>
                {
                    new Connector { Id = 1, MaxCurrentInAmps = 8 },
                }));
        var station = CreateChargeStationFake(
            parentGroup.Id,
            connectors: new List<Connector>
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 2 },
            });

        var service = BuildService(parentGroup);

        // Act / Assert
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.AddAsync(station));

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.AddAsync(station), Times.Never());
    }

    [Fact]
    public async Task ShouldNotUpdateChargeStationIfTheSumOfItsConnectorsMaxCurrentExceedsGroupsCapacity()
    {
        // Arrange
        var stationId = Guid.NewGuid().ToString();
        var parentGroup = CreateGroupFake(capacityInAmps: 10);
        parentGroup.ChargeStations.Add(
            CreateChargeStationFake(
                parentGroup.Id,
                connectors: new List<Connector>
                {
                    new Connector { Id = 1, MaxCurrentInAmps = 8 },
                }));
        var station = CreateChargeStationFake(
            parentGroup.Id,
            connectors: new List<Connector>
            {
                new Connector { Id = 1, MaxCurrentInAmps = 8 },
                new Connector { Id = 2, MaxCurrentInAmps = 3 },
            });

        var service = BuildService(parentGroup);

        // Act / Assert
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.UpdateAsync(station));

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(station), Times.Never());
    }

    private ChargeStationService BuildService(Group? parentGroup = null, ChargeStation? expectedResult = null)
    {
        _chargeStationRepositoryMock = new Mock<IChargeStationRepository>();
        _chargeStationRepositoryMock
            .Setup(m => m.AddAsync(It.IsAny<ChargeStation>()))
            .Returns(Task.FromResult(It.IsAny<ChargeStation>()));
        _chargeStationRepositoryMock
            .Setup(m => m.DeleteAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        _chargeStationRepositoryMock
            .Setup(m => m.UpdateAsync(It.IsAny<ChargeStation>()))
            .Returns(Task.FromResult(It.IsAny<ChargeStation>()));

        if (expectedResult is not null)
        {
            _chargeStationRepositoryMock
                .Setup(m => m.FindAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(expectedResult));
        }
        else
        {
            _chargeStationRepositoryMock
                .Setup(m => m.FindAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new ResourceNotFoundException("Not found."));
        }

        _groupRepositoryMock = new Mock<IGroupRepository>();
        if (parentGroup is not null)
        {
            _groupRepositoryMock
                .Setup(m => m.FindAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(parentGroup));
        }
        else
        {
            _groupRepositoryMock
                .Setup(m => m.FindAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new ResourceNotFoundException("Not found."));
        }

        return new ChargeStationService(_chargeStationRepositoryMock.Object, _groupRepositoryMock.Object);
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

    private static ChargeStation CreateChargeStationFake(string? parentGroupId = null, ICollection<Connector>? connectors = null)
    {
        return new ChargeStation
        {
            Id = Guid.NewGuid().ToString(),
            GroupId = parentGroupId ?? Guid.NewGuid().ToString(),
            Name = "Station 1",
            Connectors = connectors ?? new List<Connector>()
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 }
            }
        };
    }
}
