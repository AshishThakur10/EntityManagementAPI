namespace EntityManagementAPI.Models;

public class Entity: IEntity
{
    // public Entity(string name, bool deceased, string? gender, string id)
    //     {
    //         Name = name;
    //         Deceased = deceased;
    //         Gender = gender;
    //         Id = id;
    //     }

    public string Name { get;  set; }
    public bool Deceased { get; set; }
    public string? Gender { get; set; }
    public string Id { get; set; }
}
