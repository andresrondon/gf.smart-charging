using SmartCharging.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace SmartCharging.Api.Tests.Models
{
    public class ChargeStationTests
    {
        [Fact]
        public void ThrowsValidationExceptionWhenZeroOrNegativeMaxCurrent()
        {
            var station = new ChargeStation
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Charge Station 1",
                Connectors =
                {
                    new Connector { Id = 1, MaxCurrentInAmps = 1 },
                    new Connector { Id = 2, MaxCurrentInAmps = 1 },
                    new Connector { Id = 3, MaxCurrentInAmps = 1 },
                    new Connector { Id = 4, MaxCurrentInAmps = 1 },
                    new Connector { Id = 5, MaxCurrentInAmps = 1 },
                    new Connector { Id = 6, MaxCurrentInAmps = 1 },
                }
            };

            Assert.Throws<ValidationException>(() =>
            {
                Validator.ValidateObject(station, new ValidationContext(station), true);
            });
        }

    }
}
