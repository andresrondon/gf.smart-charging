using Moq;
using SmartCharging.Lib.Constants;
using SmartCharging.Lib.Exceptions;
using SmartCharging.Lib.Models;
using SmartCharging.Lib.Repositories.ChargeStations;
using SmartCharging.Lib.Repositories.Groups;
using SmartCharging.Lib.Services.Connectors;

namespace SmartCharging.Tests.Services;

public class ConnectorServiceTests
{
    private Mock<IChargeStationRepository> _chargeStationRepositoryMock;
    private Mock<IGroupRepository> _groupRepositoryMock;

    [Fact]
    public async Task ShouldAddNewConnector()
    {
        // Arrange
        var group = CreateGroupFake();
        var station = CreateChargeStationFake(group.Id);
        group.ChargeStations.Add(station);
        var connector = new Connector { Id = 2, MaxCurrentInAmps = 1 };
        var service = BuildService(group);

        // Act
        await service.AddAsync(connector, group.Id, station.Id);

        // Assert
        _groupRepositoryMock.Verify(m => m.FindAsync(group.Id, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(group.ChargeStations.First()), Times.Once());
    }

    [Fact]
    public async Task ShouldDeleteConnector()
    {
        // Arrange
        var parentStation = CreateChargeStationFake();
        parentStation.Connectors = new List<Connector>()
        {
            new Connector { Id = 1, MaxCurrentInAmps = 1 },
            new Connector { Id = 2, MaxCurrentInAmps = 1 }
        };
        var connectorId = 2;
        var service = BuildService(null, parentStation);

        // Act
        await service.DeleteAsync(parentStation.GroupId, parentStation.Id, connectorId);

        // Assert
        _chargeStationRepositoryMock.Verify(m => m.FindAsync(parentStation.Id, parentStation.GroupId), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(parentStation), Times.Once());

        Assert.Collection(parentStation.Connectors, item => Assert.Equal(1, item.Id));
    }

    [Fact]
    public async Task ShouldFindConnector()
    {
        // Arrange
        var parentStation = CreateChargeStationFake();
        parentStation.Connectors = new List<Connector>()
        {
            new Connector { Id = 1, MaxCurrentInAmps = 1 },
            new Connector { Id = 2, MaxCurrentInAmps = 1 }
        };
        var connectorId = 1;
        var service = BuildService(null, parentStation);

        // Act
        var result = await service.FindAsync(parentStation.GroupId, parentStation.Id, connectorId);

        _chargeStationRepositoryMock.Verify(m => m.FindAsync(parentStation.Id, parentStation.GroupId), Times.Once());
        Assert.Equal(connectorId, result.Id);
    }

    [Fact]
    public async Task ShouldUpdateConnector()
    {
        // Arrange
        var group = CreateGroupFake();
        var station = CreateChargeStationFake(group.Id);
        group.ChargeStations.Add(station);
        var newMaxCurrentValue = 2;
        var connector = new Connector { Id = 1, MaxCurrentInAmps = newMaxCurrentValue };
        var service = BuildService(parentGroup: group);

        // Act
        await service.UpdateAsync(connector, group.Id, station.Id);

        _groupRepositoryMock.Verify(m => m.FindAsync(group.Id, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(group.ChargeStations.First()), Times.Once());

        Assert.Collection(group.ChargeStations.First().Connectors, item => Assert.Equal(newMaxCurrentValue, item.MaxCurrentInAmps));
    }

    [Fact]
    public async Task ShouldNotAddConnectorIfExceedsNumberOfConnectorsPerStationLimit()
    {
        // Arrange
        var group = CreateGroupFake();
        var station = CreateChargeStationFake(
            group.Id,
            connectors: new List<Connector>()
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 1 },
                new Connector { Id = 3, MaxCurrentInAmps = 1 },
                new Connector { Id = 4, MaxCurrentInAmps = 1 },
                new Connector { Id = 5, MaxCurrentInAmps = 1 }
            });
        group.ChargeStations.Add(station);
        var connector = new Connector { Id = 6, MaxCurrentInAmps = 1 };
        var service = BuildService(parentGroup: group);

        // Act / Assert
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.AddAsync(connector, group.Id, station.Id));

        _groupRepositoryMock.Verify(m => m.FindAsync(group.Id, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(It.IsAny<ChargeStation>()), Times.Never());
    }

    [Fact]
    public async Task ShouldNotAddNewConnectorIfChargeStationDoesNotExists()
    {
        // Arrange
        var parentGroup = CreateGroupFake();
        var connector = new Connector { Id = 1, MaxCurrentInAmps = 1 };
        var service = BuildService(parentGroup, parentStation: null);

        // Act / Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => service.AddAsync(connector, parentGroup.Id, It.IsAny<string>()));

        _groupRepositoryMock.Verify(m => m.FindAsync(It.IsAny<string>(), Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(It.IsAny<ChargeStation>()), Times.Never());
    }

    [Fact]
    public async Task ShouldNotUpdateConnectorIfChargeStationDoesNotExists()
    {
        // Arrange
        var parentGroup = CreateGroupFake();
        var connector = new Connector { Id = 1, MaxCurrentInAmps = 1 };
        var service = BuildService(parentGroup, parentStation: null);

        // Act / Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => service.UpdateAsync(connector, parentGroup.Id, It.IsAny<string>()));

        _groupRepositoryMock.Verify(m => m.FindAsync(It.IsAny<string>(), Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(It.IsAny<ChargeStation>()), Times.Never());
    }

    [Fact]
    public async Task ShouldNotAddNewConnectorIfIdAlreadyExistsWithinStation()
    {
        // Arrange
        var group = CreateGroupFake();
        var station = CreateChargeStationFake(
            group.Id,
            connectors: new List<Connector>()
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 1 }
            });
        group.ChargeStations.Add(station);
        var connector = new Connector { Id = 2, MaxCurrentInAmps = 1 };
        var service = BuildService(parentGroup: group);

        // Act / Assert
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.AddAsync(connector, group.Id, station.Id));

        _groupRepositoryMock.Verify(m => m.FindAsync(group.Id, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(It.IsAny<ChargeStation>()), Times.Never());
    }

    [Fact]
    public async Task ShouldNotAddNewConnectorIfDoesNotHavePositiveMaxCurrent()
    {
        // Arrange
        var group = CreateGroupFake();
        var station = CreateChargeStationFake(
            group.Id,
            connectors: new List<Connector>()
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 1 }
            });
        group.ChargeStations.Add(station);
        var connector = new Connector { Id = 3, MaxCurrentInAmps = 0 };
        var service = BuildService(parentGroup: group);

        // Act / Assert
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.AddAsync(connector, group.Id, station.Id));

        _groupRepositoryMock.Verify(m => m.FindAsync(group.Id, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(It.IsAny<ChargeStation>()), Times.Never());
    }

    [Fact]
    public async Task ShouldNotUpdateConnectorIfDoesNotHavePositiveMaxCurrent()
    {
        // Arrange
        var group = CreateGroupFake();
        var station = CreateChargeStationFake(
            group.Id,
            connectors: new List<Connector>()
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 1 }
            });
        group.ChargeStations.Add(station);
        var connector = new Connector { Id = 2, MaxCurrentInAmps = 0 };
        var service = BuildService(parentGroup: group);

        // Act / Assert
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.UpdateAsync(connector, group.Id, station.Id));

        _groupRepositoryMock.Verify(m => m.FindAsync(group.Id, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(It.IsAny<ChargeStation>()), Times.Never());
    }

    [Fact]
    public async Task ShouldNotAddNewConnectorIfExceedsGroupCapacity()
    {
        // Arrange
        var group = CreateGroupFake(capacityInAmps: 10);
        group.ChargeStations.Add(
            new ChargeStation
            {
                Id = Guid.NewGuid().ToString(),
                GroupId = group.Id,
                Name = "Station 1",
                Connectors =
                    {
                        new Connector { Id = 1, MaxCurrentInAmps = 6 }
                    }
            });
        string stationId = Guid.NewGuid().ToString();
        group.ChargeStations.Add(
            new ChargeStation
            {
                Id = stationId,
                GroupId = group.Id,
                Name = "Station 2",
                Connectors =
                {
                    new Connector { Id = 1, MaxCurrentInAmps = 1 },
                    new Connector { Id = 2, MaxCurrentInAmps = 2 }
                }
            });
        var connector = new Connector { Id = 3, MaxCurrentInAmps = 2 };
        var service = BuildService(parentGroup: group);
        
        // Act / Assert
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.AddAsync(connector, group.Id, stationId));

        _groupRepositoryMock.Verify(m => m.FindAsync(group.Id, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(It.IsAny<ChargeStation>()), Times.Never());
    }

    [Fact]
    public async Task ShouldNotUpdateConnectorIfExceedsGroupCapacity()
    {
        // Arrange
        var group = CreateGroupFake(capacityInAmps: 10);
        group.ChargeStations.Add(
            new ChargeStation
            {
                Id = Guid.NewGuid().ToString(),
                GroupId = group.Id,
                Name = "Station 1",
                Connectors =
                    {
                        new Connector { Id = 1, MaxCurrentInAmps = 6 }
                    }
            });
        string stationId = Guid.NewGuid().ToString();
        group.ChargeStations.Add(
            new ChargeStation
            {
                Id = stationId,
                GroupId = group.Id,
                Name = "Station 2",
                Connectors =
                {
                    new Connector { Id = 1, MaxCurrentInAmps = 1 },
                    new Connector { Id = 2, MaxCurrentInAmps = 2 }
                }
            });
        var connector = new Connector { Id = 2, MaxCurrentInAmps = 4 };
        var service = BuildService(parentGroup: group);

        // Act / Assert
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.UpdateAsync(connector, group.Id, stationId));

        _groupRepositoryMock.Verify(m => m.FindAsync(group.Id, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(It.IsAny<ChargeStation>()), Times.Never());
    }

    private ConnectorService BuildService(Group? parentGroup = null, ChargeStation? parentStation = null)
    {
        _chargeStationRepositoryMock = new Mock<IChargeStationRepository>();
        _chargeStationRepositoryMock
            .Setup(m => m.UpdateAsync(It.IsAny<ChargeStation>()))
            .Returns(Task.FromResult(It.IsAny<ChargeStation>()));

        if (parentStation is not null)
        {
            _chargeStationRepositoryMock
                .Setup(m => m.FindAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(parentStation));
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

        return new ConnectorService(_chargeStationRepositoryMock.Object, _groupRepositoryMock.Object);
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
