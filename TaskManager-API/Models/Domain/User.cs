namespace TaskManager_API.Models.Domain
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        
        public string Role { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? UserImageURL { get; set; }

    }
}
