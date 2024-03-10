namespace EntityManagementAPI.Models
{
    public static class MockDatabase
    {
        public static List<Entity> Entities { get; set; } = new List<Entity>(){
            new Entity() {
                Id = "a",
                Name = "User2",
                Deceased = false,
                Gender = "Male"
            },
            new Entity() {  // "a", false,"false", "Male"
                Id = "b",
                Name = "User2",
                Deceased = false,
                Gender = "Male"
            }
        };
    }
}