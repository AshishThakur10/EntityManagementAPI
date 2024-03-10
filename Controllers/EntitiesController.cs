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
    public string CreateEntities()
    {
        return "Create a new Entities";
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