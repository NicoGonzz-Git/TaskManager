﻿namespace TaskManager_API.DTOs
{
    public class TaskResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Role { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Priority { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserLastName { get; set; }
        public string UserEmail { get; set; }
        public string UserImageURL { get; set; }

    }
}
