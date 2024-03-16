/* using EntityManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Moq;
using EntityManagement.API.Controllers;

namespace EntityManagement.Test
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test_CreateEntity_RetriesOnFailure()
            {
                // Arrange
                var mockDatabase = new Mock<IDatabase>();
                mockDatabase.Setup(db => db.SaveEntity(It.IsAny<Entity>()))
                    .Throws(new Exception("Database write failed"))
                    .Returns(true); // Simulate success after retries

                var controller = new EntitiesController(mockDatabase.Object);

                // Act
               // var entity = new Entity();
                //var response = await controller.CreateEntities(entity);

                // Assert
//                Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
  //              mockDatabase.Verify(db => db.SaveEntity(It.IsAny<Entity>()), Times.Exactly(3)); // Verify 3 retries (initial + 2 retries)
        }
        public interface IDatabase // Interface definition within the test class
       {
            bool SaveEntity(Entity entity);
        }
    }
} */