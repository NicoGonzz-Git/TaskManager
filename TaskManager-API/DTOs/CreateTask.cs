namespace TaskManager_API.DTOs
{
    public class CreateTask
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public Guid AssignedUserId { get; set; }

        public string Role { get; set; }
    }
}
