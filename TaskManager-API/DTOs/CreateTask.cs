using System.ComponentModel.DataAnnotations;

namespace TaskManager_API.DTOs
{
    public class CreateTask
    {
        [Required]
        [MaxLength(20)]
        [MinLength(1)]
        public string Title { get; set; }

        [Required]
        [MaxLength(100)]
        [MinLength(3)]
        public string Description { get; set; }

        [Required]
        public Guid AssignedUserId { get; set; }

        public string Role { get; set; }
    }
}
