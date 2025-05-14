namespace TaskManager_API.Models.Domain
{
    public class TaskItem
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public Guid AssignedUserId { get; set; }

        public string Role { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Priority { get; set; }

        public string? UserImageURL { get; set; }

        //Navigation properties
        public User User { get; set; }
    }
}
