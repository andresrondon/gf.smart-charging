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
        var parentGroup = new Group
        {
            Id = Guid.NewGuid().ToString(),
            LocationArea = Defaults.Location,
            Name = "Group 1",
            CapacityInAmps = 10
        };
        var station = new ChargeStation
        {
            Id = Guid.NewGuid().ToString(),
            GroupId = parentGroup.Id,
            Name = "Station 1",
            Connectors = new List<Connector>()
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 }
            }
        };

        var service = BuildService(parentGroup);
        await service.AddAsync(station);

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.AddAsync(station), Times.Once());
    }

    [Fact]
    public async Task ShouldDeleteChargeStation()
    {
        var stationId = Guid.NewGuid().ToString();
        var groupId = Guid.NewGuid().ToString();

        var service = BuildService();
        await service.DeleteAsync(groupId, stationId);

        _chargeStationRepositoryMock.Verify(m => m.DeleteAsync(stationId, groupId), Times.Once());
    }

    [Fact]
    public async Task ShouldFindChargeStation()
    {
        var station = new ChargeStation
        {
            Id = Guid.NewGuid().ToString(),
            GroupId = Guid.NewGuid().ToString(),
            Name = "Station 1",
            Connectors = new List<Connector>()
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 }
            }
        };

        var service = BuildService(null, station);
        await service.FindAsync(station.GroupId, station.Id);

        _chargeStationRepositoryMock.Verify(m => m.FindAsync(station.Id, station.GroupId), Times.Once());
    }

    [Fact]
    public async Task ShouldUpdateChargeStation()
    {
        var parentGroup = new Group
        {
            Id = Guid.NewGuid().ToString(),
            LocationArea = Defaults.Location,
            Name = "Group 1",
            CapacityInAmps = 10
        };
        var station = new ChargeStation
        {
            Id = Guid.NewGuid().ToString(),
            GroupId = parentGroup.Id,
            Name = "Station 1",
            Connectors = new List<Connector>()
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 }
            }
        };

        var service = BuildService(parentGroup);
        await service.UpdateAsync(station);

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(station), Times.Once());
    }

    [Fact]
    public async Task ShouldNotAddNewChargeStationWithNoConnectors()
    {
        var parentGroup = new Group
        {
            Id = Guid.NewGuid().ToString(),
            LocationArea = Defaults.Location,
            Name = "Group 1",
            CapacityInAmps = 10
        };
        var station = new ChargeStation
        {
            Id = Guid.NewGuid().ToString(),
            GroupId = parentGroup.Id,
            Name = "Station 1",
            Connectors = { }
        };

        var service = BuildService(parentGroup);
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.AddAsync(station));

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.AddAsync(station), Times.Never());
    }

    [Fact]
    public async Task ShouldNotAddNewChargeStationWithMoreThanFiveConnectors()
    {
        var parentGroup = new Group
        {
            Id = Guid.NewGuid().ToString(),
            LocationArea = Defaults.Location,
            Name = "Group 1",
            CapacityInAmps = 10
        };
        var station = new ChargeStation
        {
            Id = Guid.NewGuid().ToString(),
            GroupId = parentGroup.Id,
            Name = "Station 1",
            Connectors = 
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 1 },
                new Connector { Id = 3, MaxCurrentInAmps = 1 },
                new Connector { Id = 4, MaxCurrentInAmps = 1 },
                new Connector { Id = 5, MaxCurrentInAmps = 1 },
                new Connector { Id = 6, MaxCurrentInAmps = 1 }
            }
        };

        var service = BuildService(parentGroup);
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.AddAsync(station));

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.AddAsync(station), Times.Never());
    }

    [Fact]
    public async Task ShouldNotUpdateChargeStationWithNoConnectors()
    {
        var parentGroup = new Group
        {
            Id = Guid.NewGuid().ToString(),
            LocationArea = Defaults.Location,
            Name = "Group 1",
            CapacityInAmps = 10
        };
        var station = new ChargeStation
        {
            Id = Guid.NewGuid().ToString(),
            GroupId = parentGroup.Id,
            Name = "Station 1",
            Connectors = { }
        };

        var service = BuildService(parentGroup);
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.UpdateAsync(station));

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(station), Times.Never());
    }

    [Fact]
    public async Task ShouldNotUpdateChargeStationWithMoreThanFiveConnectors()
    {
        var parentGroup = new Group
        {
            Id = Guid.NewGuid().ToString(),
            LocationArea = Defaults.Location,
            Name = "Group 1",
            CapacityInAmps = 10
        };
        var station = new ChargeStation
        {
            Id = Guid.NewGuid().ToString(),
            GroupId = parentGroup.Id,
            Name = "Station 1",
            Connectors = 
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 1 },
                new Connector { Id = 3, MaxCurrentInAmps = 1 },
                new Connector { Id = 4, MaxCurrentInAmps = 1 },
                new Connector { Id = 5, MaxCurrentInAmps = 1 },
                new Connector { Id = 6, MaxCurrentInAmps = 1 }
            }
        };

        var service = BuildService(parentGroup);
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.UpdateAsync(station));

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(station), Times.Never());
    }

    [Fact]
    public async Task ShouldNotAddNewChargeStationIfGroupDoesNotExists()
    {
        var station = new ChargeStation
        {
            Id = Guid.NewGuid().ToString(),
            GroupId = "wrong group id",
            Name = "Station 1",
            Connectors = 
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 }
            }
        };

        var service = BuildService(parentGroup: null);
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => service.AddAsync(station));

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.AddAsync(station), Times.Never());
    }

    [Fact]
    public async Task ShouldNotUpdateChargeStationIfGroupDoesNotExists()
    {
        var station = new ChargeStation
        {
            Id = Guid.NewGuid().ToString(),
            GroupId = "wrong group id",
            Name = "Station 1",
            Connectors = 
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 }
            }
        };

        var service = BuildService(parentGroup: null);
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => service.UpdateAsync(station));

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(station), Times.Never());
    }

    [Fact]
    public async Task ShouldNotAddNewChargeStationIfConnectorsHaveDuplicateIds()
    {
        var parentGroup = new Group
        {
            Id = Guid.NewGuid().ToString(),
            LocationArea = Defaults.Location,
            Name = "Group 1",
            CapacityInAmps = 10
        };
        var station = new ChargeStation
        {
            Id = Guid.NewGuid().ToString(),
            GroupId = parentGroup.Id,
            Name = "Station 1",
            Connectors = 
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 1 }
            }
        };

        var service = BuildService(parentGroup);
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.AddAsync(station));

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.AddAsync(station), Times.Never());
    }

    [Fact]
    public async Task ShouldNotUpdateChargeStationIfConnectorsHaveDuplicateIds()
    {
        var parentGroup = new Group
        {
            Id = Guid.NewGuid().ToString(),
            LocationArea = Defaults.Location,
            Name = "Group 1",
            CapacityInAmps = 10
        };
        var station = new ChargeStation
        {
            Id = Guid.NewGuid().ToString(),
            GroupId = parentGroup.Id,
            Name = "Station 1",
            Connectors = 
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 1 }
            }
        };

        var service = BuildService(parentGroup);
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.UpdateAsync(station));

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(station), Times.Never());
    }

    [Fact]
    public async Task ShouldNotAddNewChargeStationIfConnectorsDoNotHavePositiveMaxCurrent()
    {
        var parentGroup = new Group
        {
            Id = Guid.NewGuid().ToString(),
            LocationArea = Defaults.Location,
            Name = "Group 1",
            CapacityInAmps = 10
        };
        var station = new ChargeStation
        {
            Id = Guid.NewGuid().ToString(),
            GroupId = parentGroup.Id,
            Name = "Station 1",
            Connectors =
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 0 }
            }
        };

        var service = BuildService(parentGroup);
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.AddAsync(station));

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.AddAsync(station), Times.Never());
    }

    [Fact]
    public async Task ShouldNotUpdateChargeStationIfConnectorsDoNotHavePositiveMaxCurrent()
    {
        var parentGroup = new Group
        {
            Id = Guid.NewGuid().ToString(),
            LocationArea = Defaults.Location,
            Name = "Group 1",
            CapacityInAmps = 10
        };
        var station = new ChargeStation
        {
            Id = Guid.NewGuid().ToString(),
            GroupId = parentGroup.Id,
            Name = "Station 1",
            Connectors =
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 0 }
            }
        };

        var service = BuildService(parentGroup);
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.UpdateAsync(station));

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(station), Times.Never());
    }

    [Fact]
    public async Task ShouldNotAddNewChargeStationIfTheSumOfItsConnectorsMaxCurrentExceedsGroupsCapacity()
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
                }
            }
        };
        var station = new ChargeStation
        {
            Id = Guid.NewGuid().ToString(),
            GroupId = parentGroup.Id,
            Name = "Station 2",
            Connectors = 
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 2 }
            }
        };

        var service = BuildService(parentGroup);
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.AddAsync(station));

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.AddAsync(station), Times.Never());
    }

    [Fact]
    public async Task ShouldNotUpdateChargeStationIfTheSumOfItsConnectorsMaxCurrentExceedsGroupsCapacity()
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
                }
            }
        };
        var station = new ChargeStation
        {
            Id = Guid.NewGuid().ToString(),
            GroupId = parentGroup.Id,
            Name = "Station 2",
            Connectors =
            {
                new Connector { Id = 1, MaxCurrentInAmps = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 2 }
            }
        };

        var service = BuildService(parentGroup);
        await Assert.ThrowsAsync<BusinessRulesValidationException>(() => service.UpdateAsync(station));

        _groupRepositoryMock.Verify(m => m.FindAsync(station.GroupId, Defaults.Location), Times.Once());
        _chargeStationRepositoryMock.Verify(m => m.UpdateAsync(station), Times.Never());
    }

    private ChargeStationService BuildService(Group? parentGroup = null, ChargeStation? expectedResult = null)
    {
        _chargeStationRepositoryMock = new Mock<IChargeStationRepository>();
        _chargeStationRepositoryMock
            .Setup(m => m.AddAsync(It.IsAny<ChargeStation>()))
            .Returns(Task.CompletedTask);
        _chargeStationRepositoryMock
            .Setup(m => m.DeleteAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        _chargeStationRepositoryMock
            .Setup(m => m.UpdateAsync(It.IsAny<ChargeStation>()))
            .Returns(Task.CompletedTask);

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
}
