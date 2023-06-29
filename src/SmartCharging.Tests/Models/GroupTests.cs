﻿using SmartCharging.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace SmartCharging.Api.Tests.Models
{
    public class GroupTests
    {
        [Fact]
        public void ThrowsValidationExceptionWhenZeroOrNegativeCapacity()
        {
            var group = new Group
            {
                Id = Guid.NewGuid().ToString(),
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