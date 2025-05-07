using System.ComponentModel.DataAnnotations;

namespace TaskManager_API.DTOs
{
    public class CreateUser
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Role { get; set; }
        public string? UserImageURL { get; set; }
    }
}
