using SmartCharging.Lib.Models;
using System.ComponentModel.DataAnnotations;

namespace SmartCharging.Api.Tests.Models;

public class ConnectorTests
{
    [Fact]
    public void ThrowsValidationExceptionWhenZeroOrNegativeMaxCurrent()
    {
        var connector = new Connector
        {
            Id = 1,
            MaxCurrentInAmps = 0
        };

        Assert.Throws<ValidationException>(() =>
        {
            Validator.ValidateObject(connector, new ValidationContext(connector), true);
        });
    }

}
