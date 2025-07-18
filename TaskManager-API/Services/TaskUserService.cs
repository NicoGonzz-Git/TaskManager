﻿using Microsoft.EntityFrameworkCore;
using TaskManager_API.Data;
using TaskManager_API.Models.Domain;
using TaskManager_API.DTOs;
using System.Threading.Tasks;

namespace TaskManager_API.Services
{
    public class TaskUserService
    {
        private readonly TaskContext _taskContext;
        private readonly UserContext _userContext;
        private readonly ILogger<TaskUserService> _logger;

        public TaskUserService(TaskContext taskContext, UserContext userContext, ILogger<TaskUserService> logger)
        {
            _taskContext = taskContext;
            _userContext = userContext;
            _logger = logger;
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByUserEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Attempted to get tasks with null or empty email");
                return new List<TaskItem>();
            }

            try
            {
                var user = await _userContext.Users
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    _logger.LogWarning($"User with email {email} not found");
                    return new List<TaskItem>();
                }

                return await _taskContext.Tasks
                    .Where(t => t.AssignedUserId == user.Id)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting tasks for user with email {email}");
                throw;
            }
        }

        public async Task<List<TasksDto>> GetAllTasksAsync()
        {
            var tasks = await _taskContext.Tasks.ToListAsync();
            var userIds = tasks.Select(t => t.AssignedUserId).Distinct().ToList();

            var users = await _userContext.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            var result = tasks.Select(task =>
            {
                users.TryGetValue(task.AssignedUserId, out var user);

                return new TasksDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Role = task.Role,
                    CreatedDate = task.CreatedDate,
                    UserName = user?.Name ?? "",
                    UserLastName = user?.LastName ?? "",
                    UserEmail = user?.Email ?? "",
                    UserImageURL = user?.UserImageURL ?? "",
                    Priority= task.Priority
                };
            }).ToList();

            return result;
        }

        public async Task<TaskItem> GetTaskByIdAsync(Guid taskId)
        {
            var task = await _taskContext.Tasks.FindAsync(taskId);
            if (task == null)
            {
                return null;
            }

            var user = await _userContext.Users.FindAsync(task.AssignedUserId);
            if (user != null)
            {
                task.User = user;
            }

            return task;
        }

        public async Task<TaskResponseDto> CreateTaskAsync(CreateTask createTask)
        {
            var user = await _userContext.Users.FindAsync(createTask.AssignedUserId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {createTask.AssignedUserId} not found");
            }

            var taskItem = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = createTask.Title,
                Description = createTask.Description,
                AssignedUserId = createTask.AssignedUserId,
                Role = createTask.Role,
                CreatedDate = createTask.CreatedDate,
                UserImageURL = user.UserImageURL,
                Priority = createTask.Priority
            };

            await _taskContext.Tasks.AddAsync(taskItem);
            await _taskContext.SaveChangesAsync();

            return new TaskResponseDto
            {
                Id = taskItem.Id,
                Title = taskItem.Title,
                Description = taskItem.Description,
                Role = taskItem.Role,
                CreatedDate = taskItem.CreatedDate,
                UserId = user.Id,
                UserName = user.Name,
                UserLastName = user.LastName,
                UserEmail = user.Email,
                UserImageURL = user.UserImageURL,
                Priority = taskItem.Priority
            };
        }
    }
}