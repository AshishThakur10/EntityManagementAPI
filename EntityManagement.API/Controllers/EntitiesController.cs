using EntityManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Polly;


namespace EntityManagement.API.Controllers
{

    public enum SortDirection
    {
        Ascending,
        Descending
    }

    [ApiController]
    [Route("api/[controller]")]
    public class EntitiesController : ControllerBase
    {
        private readonly ILogger<EntitiesController> _logger;
        public EntitiesController(ILogger<EntitiesController> logger)
        {
            _logger = logger;
        }

        private const int MaxRetryAttempts = 3;
        private static readonly TimeSpan InitialRetryDelay = TimeSpan.FromSeconds(2);
        private static readonly TimeSpan MaxRetryDelay = TimeSpan.FromSeconds(30);

        private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(2);

        [HttpPost]
        [Route("", Name = "CreateEntity")]
        public async Task<ActionResult> CreateEntities(Entity entity)
        {
            try
            {
                // Log information before entity creation
                _logger.LogInformation("Attempting to create entity...");

                // Check if the provided entity is valid
                if (!ModelState.IsValid) return BadRequest(ModelState);// 400 Bad Request if the model state is not valid

                // Retry mechanism for database write operation
                var policy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(MaxRetryAttempts, retryAttempt => {
                        TimeSpan delay = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                        //Logger for retry and delay Attempt
                        _logger.LogInformation("Retry attempt {RetryAttempt}, delaying for {Delay} seconds...", retryAttempt, delay.TotalSeconds);
                        return delay;
                    }
                    );

                return await policy.ExecuteAsync(async () =>
                {
                    // Generate a unique ID for the new entity
                    string newEntityId = Guid.NewGuid().ToString();

                    // Set the ID of the provided entity to the newly generated ID
                    entity.Id = newEntityId;

                    // Add the new entity to the mock database
                    MockDatabase.Entities.Add(entity);
                    
                    // Log information after entity creation
                    _logger.LogInformation("Entity created successfully with ID: {EntityId}", newEntityId);

                    // Return the ID and the result (whole entity) of the newly created entity
                    return Ok(entity);
                });
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while creating the entity.");
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

        [HttpGet("{Id}", Name = "GetEntitiyByID")]
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
        [Route("{Id}", Name = "UpdateById")]
        public async Task<ActionResult<Entity>> UpdateEntities(string Id, Entity entity)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                // Retry mechanism with exponential backoff for database write operation
                int retryAttempt = 0;
                TimeSpan delay = InitialRetryDelay;

                while (retryAttempt < MaxRetryAttempts)
                {
                    try
                    {
                        var entityToUpdate = MockDatabase.Entities.FirstOrDefault(n => n.Id == Id);
                        if (entityToUpdate == null)
                        {
                            delay = TimeSpan.FromSeconds(Math.Min(Math.Pow(2, retryAttempt) + new Random().NextDouble(), MaxRetryDelay.TotalSeconds));
                            Thread.Sleep(delay); return NotFound($"Entity with ID {Id} does not exist.");  // 404 Status Code
                        }
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
                    catch (Exception)
                    {
                        retryAttempt++;
                        if (retryAttempt < MaxRetryAttempts)
                        {
                            // Exponential backoff delay
                            // delay = TimeSpan.FromSeconds(Math.Min(Math.Pow(2, retryAttempt) + new Random().NextDouble(), MaxRetryDelay.TotalSeconds));
                            // Thread.Sleep(delay);
                            Thread.Sleep(RetryDelay * retryAttempt); // Exponential backoff delay
                        }
                    }
                }
                // Return error response if all retry attempts fail
                return StatusCode(500, "Failed to update entity after multiple retry attempts.");

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the entity. {ex}"); // 500 Internal Server Error
            }
        }
        
        // DELETE method to delete entities by ID
        [HttpDelete]
        [Route("{Id:alpha}", Name = "DeleteById")]
        public ActionResult DeleteEntities(string Id)
        {
            try
            {
                // Log that the deletion process is initiated
                _logger.LogInformation($"Attempting to delete entity with ID: {Id}");

                // Find the entity to delete by ID
                var entityToDelete = MockDatabase.Entities.FirstOrDefault(n => n.Id == Id);
                if (entityToDelete == null)
                {
                    // Log if the entity with the provided ID does not exist
                    _logger.LogInformation($"Entity with ID {Id} does not exist.");
                    return NotFound($"Entity with ID {Id} does not exist.");
                }
                
                // Remove the entity from the database
                MockDatabase.Entities.Remove(entityToDelete);

                // Log that the entity is successfully deleted
                _logger.LogInformation($"Entity with ID {Id} successfully deleted.");
                return NoContent();
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the deletion process
                _logger.LogError(ex, $"Failed to delete entity with ID {Id}. Please try again later.");
                return StatusCode(500, $"Failed to delete entity with ID {Id}. Please try again later. {ex}");
            }
        }
    }
}
