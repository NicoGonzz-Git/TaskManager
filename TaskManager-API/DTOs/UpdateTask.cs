namespace TaskManager_API.DTOs
{
    public class UpdateTask
    {
        public string Title { get; set; }
        public string Description { get; set; } 
        public string Priority {  get; set; }
        public Guid AssignedUserId { get; set; }
    }
}
