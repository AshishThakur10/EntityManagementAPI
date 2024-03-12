using EntityManagementAPI;
using EntityManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;

public enum SortDirection
{
    Ascending,
    Descending
}

[ApiController]
[Route("api/[controller]")]
public class EntitiesController : ControllerBase
{

    [HttpPost]
    [Route("", Name = "CreateEntity")]
    public async Task<ActionResult<Entity>> CreateEntities(Entity entity)
    {
        try
        {
            // Check if the provided entity is valid
            if (!ModelState.IsValid) return BadRequest(ModelState); // 400 Bad Request if the model state is not valid

            // Generate a unique ID for the new entity
            string newEntityId = Guid.NewGuid().ToString();

            // Set the ID of the provided entity to the newly generated ID
            entity.Id = newEntityId;

            // Add the new entity to the mock database
            MockDatabase.Entities.Add(entity);

            // Return the ID of the newly created entity
            return Ok(entity);//CreatedAtRoute("GetEntityByID", new { id = newEntityId }, entity); // 201 Created with the entity data
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, $"An error occurred while creating the entity. {ex}"); // 500 Internal Server Error
        }
    }

    [HttpGet]
    [Route("All", Name = "GetAllEntities")]
    public ActionResult<IEnumerable<Entity>> GetAllEntities(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] SortDirection sortDirection = SortDirection.Ascending,
        [FromQuery] string? search = null,
        [FromQuery] string? gender = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string[]? countries = null)
    {
        try
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Invalid pagination parameters. pageNumber and pageSize must be positive integers.");
            }

            var entities = MockDatabase.Entities.AsEnumerable(); // Convert to IEnumberable for sorting

            // Apply Filters (if provided)
            if (!string.IsNullOrEmpty(gender))
            {
                entities = entities.Where(e => e.Gender.ToLowerInvariant() == gender.ToLowerInvariant());
            }
            if (startDate.HasValue && endDate.HasValue)
            {
                entities = entities.Where(e => e.Dates.Any(d => d.DateType == "Birth Date" && d.Dates >= startDate && d.Dates <= endDate));
            }

            if (countries?.Any() == true)
            {
                entities = entities.Where(e => e.Addresses.Any(a => countries.Contains(a.Country.ToLowerInvariant())));
            }


            //Apply Searching (if provided)
            if (!string.IsNullOrEmpty(search))
            {
                entities = entities.Where(e =>
                    e.Names.Any(n => n.FirstName.ToLowerInvariant().Contains(search.ToLowerInvariant())) ||
                    e.Names.Any(n => n.MiddleName?.ToLowerInvariant().Contains(search.ToLowerInvariant()) ?? false) ||
                    e.Names.Any(n => n.Surname.ToLowerInvariant().Contains(search.ToLowerInvariant())) ||
                    e.Addresses.Any(a => a.AddressLine.ToLowerInvariant().Contains(search.ToLowerInvariant())) ||
                    e.Addresses.Any(a => a.City.ToLowerInvariant().Contains(search.ToLowerInvariant())) ||
                    e.Addresses.Any(a => a.Country.ToLowerInvariant().Contains(search.ToLowerInvariant())));
            }

            // Apply Sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLowerInvariant())
                {
                    case "name":
                        entities = sortDirection == SortDirection.Ascending
                            ? entities.OrderBy(e => e.Names.FirstOrDefault()?.FirstName)
                            : entities.OrderByDescending(e => e.Names.FirstOrDefault()?.FirstName);
                        break;
                    case "deceased":
                        entities = sortDirection == SortDirection.Ascending
                            ? entities.OrderBy(e => e.Deceased)
                            : entities.OrderByDescending(e => e.Deceased);
                        break;
                    case "gender":
                        entities = sortDirection == SortDirection.Ascending
                            ? entities.OrderBy(e => e.Gender)
                            : entities.OrderByDescending(e => e.Gender);
                        break;
                    default:
                        return BadRequest("sorting query invalid choose from name, deceased, gender");
                        break;
                }
            }

            int totalEntities = entities.Count();
            int totalPages = (int)Math.Ceiling((double)totalEntities / pageSize);

            if (pageNumber > totalPages)
            {
                return NotFound("The requested page number is greater than the total number of pages.");
            }
            var paginatedEntities = entities
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            var response = new
            {
                Data = paginatedEntities,
                PageNumber = pageNumber,
                pageSize = pageSize,
                TotalCount = totalEntities
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching entities. {ex}");
        }
    }

    [HttpGet("{Id:alpha}", Name = "GetEntitiyByID")]
    public ActionResult<Entity> GetEntitiyByID(string Id)
    {
        try
        {
            var data = MockDatabase.Entities.FirstOrDefault(e => e.Id == Id);
            if (data == null)
            {
                return NotFound($"Entity with ID {Id} does not exist."); // 404 Not Found
            }

            return Ok(data); // 200 OK with data
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, $"An error occurred while fetching the entity. {ex}"); // 500 Internal Server Error
        }
    }

    [HttpPut]
    [Route("{Id:alpha}", Name = "UpdateById")]
    public async Task<ActionResult<Entity>> UpdateEntities(string Id, Entity entity)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var entityToUpdate = MockDatabase.Entities.FirstOrDefault(n => n.Id == Id);
            if (entityToUpdate == null) return NotFound($"Entity with ID {Id} does not exist.");  // 404 Status Code

            // Update the properties of entityToUpdate with the properties of the provided entity
            entityToUpdate.Deceased = entity.Deceased != null ? entity.Deceased : entityToUpdate.Deceased;
            entityToUpdate.Gender = !string.IsNullOrEmpty(entity.Gender) ? entity.Gender : entityToUpdate.Gender;
            entityToUpdate.Addresses = entity.Addresses != null ? entity.Addresses : entityToUpdate.Addresses;
            entityToUpdate.Dates = entity.Dates != null ? entity.Dates : entityToUpdate.Dates;
            entityToUpdate.Names = entity.Names != null ? entity.Names : entityToUpdate.Names;

            // In a real scenario with Entity Framework, you would call SaveChanges here
            // MockDatabase.SaveChanges();
            return NoContent(); //204 no content successful update
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the entity. {ex}"); // 500 Internal Server Error
        }
    }

    [HttpDelete]
    [Route("{Id:alpha}", Name = "DeleteById")]
    public ActionResult DeleteEntities(string Id)
    {
        try
        {
            var entityToDelete = MockDatabase.Entities.FirstOrDefault(n => n.Id == Id);
            if (entityToDelete == null)
                return NotFound($"Entity with ID {Id} does not exist.");
            MockDatabase.Entities.Remove(entityToDelete);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to delete entity with ID {Id}. Please try again later. {ex}");
        }
    }
}