using System;
using System.Collections.Generic;
using EntityManagement.API;
using EntityManagement.API.Controllers;
using Microsoft.Extensions.Logging;

using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EntityManagement.Test
{
    public class WTest
    {

        [Fact]
        public void Get_ReturnsRandomizedWeatherData()
        {
            // Arrange (Setup)
            var mockLogger = new Mock<ILogger<WeatherForecastController>>();
            var controller = new WeatherForecastController(mockLogger.Object);

            // Act (Simulate the action)
            var firstResult = controller.Get().ToList();
            var secondResult = controller.Get().ToList();

            // Assert (Verify the randomness)
            Assert.False(firstResult.SequenceEqual(secondResult)); // Ensure results differ due to randomness
        }
    }
}
