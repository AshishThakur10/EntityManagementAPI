using EntityManagementAPI;
using EntityManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EntitiesController: ControllerBase{

    [HttpPost]
    public string CreateEntities(){
        return "Create a new Entities";
    }

    [HttpGet]
    [Route("All", Name = "GetAllEntities")]
    public IEnumerable<Entity> GetAllEntities(){
        return MockDatabase.Entities;
    }

    [HttpGet("{Id:alpha}", Name = "GetEntitiyByID")]
    public Entity GetEntitiyByID(string Id){
        var data = MockDatabase.Entities.FirstOrDefault(e => e.Id == Id);
        return data;
    }

    [HttpPut]
    [Route("{Id:alpha}")]
    public string UpdateEntities(string Id){
        return "Update Entities by ID "+ Id;
    }

    [HttpDelete]
    [Route("{Id:alpha}")]
    public string DeleteEntities(string Id){
        var data = MockDatabase.Entities.FirstOrDefault(n => n.Id == Id);
        MockDatabase.Entities.Remove(data);
        return "Successfully Deleted" + Id;
    }
}