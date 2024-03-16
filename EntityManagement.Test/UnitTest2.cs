using EntityManagement.API.Controllers;
using EntityManagementAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EntityManagement.Test
{
    public class UnitTest2
    {

        [Fact]
        public void CreateEntities_DatabaseWriteOperationSuccessfulOnFirstAttempt_Returns200OkResponse()
        {
            // Arrange
            var controller = new EntitiesController();
            var entity = new Entity
            {
                Id = "test-entity",
                Dates = new List<Date>(),
                Names = new List<Name>()
            };

            // Act
            var result = controller.CreateEntities(entity);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var createdEntity = Assert.IsAssignableFrom<Entity>(okResult.Value);
            Assert.Equal(entity.Id, createdEntity.Id);
            Assert.NotNull(createdEntity.Dates);

        }
    }
}