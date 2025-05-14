using System.ComponentModel.DataAnnotations;

namespace TaskManager_API.DTOs
{
    public class CreateTask
    {
        [Required]
        [MaxLength(30)]
        [MinLength(1)]
        public string Title { get; set; }

        [Required]
        [MaxLength(150)]
        [MinLength(3)]
        public string Description { get; set; }

        [Required]
        public Guid AssignedUserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Priority { get; set; }
        public string Role { get; set; }
    }
}
