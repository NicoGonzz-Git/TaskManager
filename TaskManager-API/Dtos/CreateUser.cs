namespace TaskManager_API.Dtos
{
    public class CreateUser
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public string? UserImageURL { get; set; }
    }
}
