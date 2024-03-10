namespace EntityManagementAPI.Models
{
    public static class MockDatabase
    {
        public static List<Entity> Entities { get; set; } = new List<Entity>(){
            new Entity
            {
                Id = "a",
                Names = new List<Name>() { new Name { FirstName = "User1", MiddleName = "", Surname = "" } },
                Deceased = false,
                Gender = "Male",
                Addresses = new List<Address>() { new Address { AddressLine = "", City = "", Country = "" } },
                Dates = new List<Date>() { new Date { DateType = "", Dates = DateTime.UtcNow } } 
            },
            new Entity
            {
                Id = "b",
                Names = new List<Name>() { new Name { FirstName = "User2", MiddleName = "", Surname = "" } },
                Deceased = false,
                Gender = "Male",
                Addresses = new List<Address>() { new Address { AddressLine = "", City = "", Country = "" } },
                Dates = new List<Date>() { new Date { DateType = "", Dates = DateTime.UtcNow } }
            },
            new Entity
            {
                Id = Guid.NewGuid().ToString(),
                Names = new List<Name>() { new Name { FirstName = "David Lee", MiddleName = "", Surname = "" } },
                Deceased = false,
                Gender = "Male",
                Addresses = new List<Address>() { new Address { AddressLine = "789 Park Ave", City = "Chicago", Country = "USA" } },
                Dates = new List<Date>() { new Date { DateType = "Birth Date", Dates = DateTime.UtcNow.AddYears(-20) } }
            }
        };
    }
}