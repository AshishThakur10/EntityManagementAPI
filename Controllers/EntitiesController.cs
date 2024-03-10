using EntityManagementAPI;
using EntityManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EntitiesController : ControllerBase
{

    [HttpPost]
    public string CreateEntities()
    {
        return "Create a new Entities";
    }

    [HttpGet]
    [Route("All", Name = "GetAllEntities")]
    public ActionResult<IEnumerable<Entity>> GetAllEntities([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            return BadRequest("Invalid pagination parameters. pageNumber and pageSize must be positive integers.");
        }
        int totalEntities = MockDatabase.Entities.Count();
        int totalPages = (int)Math.Ceiling((double)totalEntities / pageSize);

        if (pageNumber > totalPages)
        {
            return NotFound("The requested page number is greater than the total number of pages.");
        }
        var paginatedEntities = MockDatabase.Entities
        .Skip((pageNumber-1) * pageSize)
        .Take(pageSize)
        .ToList();

        var response = new {
            Data = paginatedEntities,
            PageNumber = pageNumber,
            pageSize  = pageSize,
            TotalCount = totalEntities
        };

        return Ok(response);
    }

    [HttpGet("{Id:alpha}", Name = "GetEntitiyByID")]
    public ActionResult<Entity> GetEntitiyByID(string Id)
    {
        var data = MockDatabase.Entities.FirstOrDefault(e => e.Id == Id);
        return data;
    }

    [HttpPut]
    [Route("{Id:alpha}", Name = "UpdateById")]
    public string UpdateEntities(string Id)
    {
        return "Update Entities by ID " + Id;
    }

    [HttpDelete]
    [Route("{Id:alpha}", Name = "DeleteById")]
    public string DeleteEntities(string Id)
    {
        var data = MockDatabase.Entities.FirstOrDefault(n => n.Id == Id);
        MockDatabase.Entities.Remove(data);
        return "Successfully Deleted" + Id;
    }
}