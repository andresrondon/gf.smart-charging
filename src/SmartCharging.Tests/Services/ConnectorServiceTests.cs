using Moq;
using SmartCharging.Lib.Constants;
using SmartCharging.Lib.Exceptions;
using SmartCharging.Lib.Models;
using SmartCharging.Lib.Repositories.ChargeStations;
using SmartCharging.Lib.Repositories.Groups;
using SmartCharging.Lib.Services.ChargeStations;
using SmartCharging.Lib.Services.Connectors;

namespace SmartCharging.Tests.Services;

public class ConnectorServiceTests
{
    private Mock<IChargeStationRepository> _chargeStationRepositoryMock;
    private Mock<IGroupRepository> _groupRepositoryMock;

    [Fact]
    public async Task ShouldAddNewConnector()
    {
        var groupId = Guid.NewGuid().ToString();
        var stationId = Guid.NewGuid().ToString();
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
                    Id = stationId,
                    GroupId = groupId,
                    Name = "Station 1",
                    Connectors = new List<Connector>()
                    {
                        new Connector { Id = 1, MaxCurrentInAmps = 1 }
                    }
                }
            }
        };
        var connector = new Connector { Id = 2, MaxCurrentInAmps = 1 };

        var service = BuildService(parentGroup);
        await service.AddAsync(connector, groupId, stationId);

        _groupRepositoryMock.Verify(m => m.FindAsync(groupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(parentGroup.ChargeStations.First()), Times.Once());
    }

    [Fact]
    public async Task ShouldDeleteConnector()
    {
        var parentStation = new ChargeStation
        {
            Id = Guid.NewGuid().ToString(),
            GroupId = Guid.NewGuid().ToString(),
            Name = "Station 1",
            Connectors = new List<Connector>()
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 1 }
            }
        };

        var connectorId = 2;

        var service = BuildService(null, parentStation);
        await service.DeleteAsync(parentStation.GroupId, parentStation.Id, connectorId);

        _chargeStationRepositoryMock.Verify(m => m.FindAsync(parentStation.Id, parentStation.GroupId), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(parentStation), Times.Once());

        Assert.Collection(parentStation.Connectors, item => Assert.Equal(1, item.Id));
    }

    [Fact]
    public async Task ShouldFindConnector()
    {
        var parentStation = new ChargeStation
        {
            Id = Guid.NewGuid().ToString(),
            GroupId = Guid.NewGuid().ToString(),
            Name = "Station 1",
            Connectors = new List<Connector>()
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 1 }
            }
        };
        var connectorId = 1;

        var service = BuildService(null, parentStation);
        var result = await service.FindAsync(parentStation.GroupId, parentStation.Id, connectorId);

        _chargeStationRepositoryMock.Verify(m => m.FindAsync(parentStation.Id, parentStation.GroupId), Times.Once());
        Assert.Equal(connectorId, result.Id);
    }

    [Fact]
    public async Task ShouldUpdateConnector()
    {
        var groupId = Guid.NewGuid().ToString();
        var stationId = Guid.NewGuid().ToString();
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
                    Id = stationId,
                    GroupId = groupId,
                    Name = "Station 1",
                    Connectors = new List<Connector>()
                    {
                        new Connector { Id = 1, MaxCurrentInAmps = 1 }
                    }
                }
            }
        };
        var newMaxCurrentValue = 2;
        var connector = new Connector { Id = 1, MaxCurrentInAmps = newMaxCurrentValue };

        var service = BuildService(parentGroup);
        await service.UpdateAsync(connector, groupId, stationId);

        _groupRepositoryMock.Verify(m => m.FindAsync(groupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(parentGroup.ChargeStations.First()), Times.Once());

        Assert.Collection(parentGroup.ChargeStations.First().Connectors, item => Assert.Equal(newMaxCurrentValue, item.MaxCurrentInAmps));
    }

    [Fact]
    public async Task ShouldNotAddConnectorIfExceedsNumberOfConnectorsPerStationLimit()
    {
        var groupId = Guid.NewGuid().ToString();
        var stationId = Guid.NewGuid().ToString();
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
                    Id = stationId,
                    GroupId = groupId,
                    Name = "Station 1",
                    Connectors = new List<Connector>()
                    {
                        new Connector { Id = 1, MaxCurrentInAmps = 1 },
                        new Connector { Id = 2, MaxCurrentInAmps = 1 },
                        new Connector { Id = 3, MaxCurrentInAmps = 1 },
                        new Connector { Id = 4, MaxCurrentInAmps = 1 },
                        new Connector { Id = 5, MaxCurrentInAmps = 1 }
                    }
                }
            }
        };
        var connector = new Connector { Id = 6, MaxCurrentInAmps = 1 };

        var service = BuildService(parentGroup);
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.AddAsync(connector, groupId, stationId));

        _groupRepositoryMock.Verify(m => m.FindAsync(groupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(It.IsAny<ChargeStation>()), Times.Never());
    }

    [Fact]
    public async Task ShouldNotAddNewConnectorIfChargeStationDoesNotExists()
    {
        var parentGroup = new Group
        {
            Id = Guid.NewGuid().ToString(),
            LocationArea = Defaults.Location,
            Name = "Group 1",
            CapacityInAmps = 10,
            ChargeStations = { }
        };
        var connector = new Connector { Id = 1, MaxCurrentInAmps = 1 };

        var service = BuildService(parentGroup, parentStation: null);

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => service.AddAsync(connector, parentGroup.Id, It.IsAny<string>()));

        _groupRepositoryMock.Verify(m => m.FindAsync(It.IsAny<string>(), Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(It.IsAny<ChargeStation>()), Times.Never());
    }

    [Fact]
    public async Task ShouldNotUpdateConnectorIfChargeStationDoesNotExists()
    {
        var parentGroup = new Group
        {
            Id = Guid.NewGuid().ToString(),
            LocationArea = Defaults.Location,
            Name = "Group 1",
            CapacityInAmps = 10,
            ChargeStations = { }
        };
        var connector = new Connector { Id = 1, MaxCurrentInAmps = 1 };

        var service = BuildService(parentGroup, parentStation: null);

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => service.UpdateAsync(connector, parentGroup.Id, It.IsAny<string>()));

        _groupRepositoryMock.Verify(m => m.FindAsync(It.IsAny<string>(), Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(It.IsAny<ChargeStation>()), Times.Never());
    }

    [Fact]
    public async Task ShouldNotAddNewConnectorIfIdAlreadyExistsWithinStation()
    {
        var groupId = Guid.NewGuid().ToString();
        var stationId = Guid.NewGuid().ToString();
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
                    Id = stationId,
                    GroupId = groupId,
                    Name = "Station 1",
                    Connectors = new List<Connector>()
                    {
                        new Connector { Id = 1, MaxCurrentInAmps = 1 },
                        new Connector { Id = 2, MaxCurrentInAmps = 1 },
                    }
                }
            }
        };
        var connector = new Connector { Id = 2, MaxCurrentInAmps = 1 };

        var service = BuildService(parentGroup);
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.AddAsync(connector, groupId, stationId));

        _groupRepositoryMock.Verify(m => m.FindAsync(groupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(It.IsAny<ChargeStation>()), Times.Never());
    }

    [Fact]
    public async Task ShouldNotAddNewConnectorIfDoesNotHavePositiveMaxCurrent()
    {
        var groupId = Guid.NewGuid().ToString();
        var stationId = Guid.NewGuid().ToString();
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
                    Id = stationId,
                    GroupId = groupId,
                    Name = "Station 1",
                    Connectors = new List<Connector>()
                    {
                        new Connector { Id = 1, MaxCurrentInAmps = 1 },
                        new Connector { Id = 2, MaxCurrentInAmps = 1 },
                    }
                }
            }
        };
        var connector = new Connector { Id = 3, MaxCurrentInAmps = 0 };

        var service = BuildService(parentGroup);
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.AddAsync(connector, groupId, stationId));

        _groupRepositoryMock.Verify(m => m.FindAsync(groupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(It.IsAny<ChargeStation>()), Times.Never());
    }

    [Fact]
    public async Task ShouldNotUpdateConnectorIfDoesNotHavePositiveMaxCurrent()
    {
        var groupId = Guid.NewGuid().ToString();
        var stationId = Guid.NewGuid().ToString();
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
                    Id = stationId,
                    GroupId = groupId,
                    Name = "Station 1",
                    Connectors = new List<Connector>()
                    {
                        new Connector { Id = 1, MaxCurrentInAmps = 1 },
                        new Connector { Id = 2, MaxCurrentInAmps = 1 },
                    }
                }
            }
        };
        var connector = new Connector { Id = 2, MaxCurrentInAmps = 0 };

        var service = BuildService(parentGroup);
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.UpdateAsync(connector, groupId, stationId));

        _groupRepositoryMock.Verify(m => m.FindAsync(groupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(It.IsAny<ChargeStation>()), Times.Never());
    }

    [Fact]
    public async Task ShouldNotAddNewConnectorIfExceedsGroupCapacity()
    {
        string groupId = Guid.NewGuid().ToString();
        string stationId = Guid.NewGuid().ToString();
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
                    Id = Guid.NewGuid().ToString(),
                    GroupId = groupId,
                    Name = "Station 1",
                    Connectors =
                    {
                        new Connector { Id = 1, MaxCurrentInAmps = 6 }
                    }
                },
                new ChargeStation
                {
                    Id = stationId,
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
        var connector = new Connector { Id = 3, MaxCurrentInAmps = 2 };

        var service = BuildService(parentGroup);
        
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.AddAsync(connector, groupId, stationId));

        _groupRepositoryMock.Verify(m => m.FindAsync(groupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(It.IsAny<ChargeStation>()), Times.Never());
    }

    [Fact]
    public async Task ShouldNotUpdateConnectorIfExceedsGroupCapacity()
    {
        string groupId = Guid.NewGuid().ToString();
        string stationId = Guid.NewGuid().ToString();
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
                    Id = Guid.NewGuid().ToString(),
                    GroupId = groupId,
                    Name = "Station 1",
                    Connectors =
                    {
                        new Connector { Id = 1, MaxCurrentInAmps = 6 }
                    }
                },
                new ChargeStation
                {
                    Id = stationId,
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
        var connector = new Connector { Id = 2, MaxCurrentInAmps = 4 };

        var service = BuildService(parentGroup);

        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.UpdateAsync(connector, groupId, stationId));

        _groupRepositoryMock.Verify(m => m.FindAsync(groupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(It.IsAny<ChargeStation>()), Times.Never());
    }

    private ConnectorService BuildService(Group? parentGroup = null, ChargeStation? parentStation = null)
    {
        _chargeStationRepositoryMock = new Mock<IChargeStationRepository>();
        _chargeStationRepositoryMock
            .Setup(m => m.UpdateAsync(It.IsAny<ChargeStation>()))
            .Returns(Task.CompletedTask);

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
}
