using EntityManagement.API.Controllers;
using EntityManagementAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Polly;

namespace EntityManagement.Test
{
    public interface IDatabase
    {
        bool CreateEntity(Entity entity);
    }

    public class UnitTest3
    {
        [Fact]
        public async Task CreateEntity_RetryPolicy_Success()
        {
            // Arrange
            var entity = new Entity
            {
                Id = "test-entity",
                Dates = new List<Date>(),
                Names = new List<Name>()
            };
            var mockDatabase = new Mock<IDatabase>();

            var controller = new EntityController(mockDatabase.Object);

            // Create a Polly retry policy with 3 retry attempts and a 2-second delay between each attempt
            var policy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2));

            // Act
            var result = await policy.ExecuteAsync(async () =>
            {
                var okResult = await controller.CreateEntity(entity);
                return okResult.Value;
            });

            // Assert
            Assert.True(result);
            mockDatabase.Verify(x => x.CreateEntity(entity), Times.Exactly(3));
        }
    }
}