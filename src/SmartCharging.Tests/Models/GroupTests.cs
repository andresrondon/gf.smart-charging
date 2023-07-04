using SmartCharging.Lib.Constants;
using SmartCharging.Lib.Models;
using System.ComponentModel.DataAnnotations;

namespace SmartCharging.Tests.Models
{
    public class GroupTests
    {
        [Fact]
        public void ThrowsValidationExceptionWhenZeroOrNegativeCapacity()
        {
            var group = new Group
            {
                Id = Guid.NewGuid().ToString(),
                LocationArea = Defaults.Location,
                Name = "Group 1",
                CapacityInAmps = 0
            };

            Assert.Throws<ValidationException>(() =>
            {
                Validator.ValidateObject(group, new ValidationContext(group), true);
            });
        }
    }
}
