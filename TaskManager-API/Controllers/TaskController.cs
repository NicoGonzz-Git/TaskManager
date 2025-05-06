using Microsoft.AspNetCore.Mvc;
using TaskManager_API.Data;
using TaskManager_API.Services;
using TaskManager_API.DTOs;

namespace TaskManager_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly TaskContext _context;
        private readonly TaskUserService _taskService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(
            TaskContext context,
            TaskUserService taskService,
            ILogger<TasksController> logger)
        {
            _context = context;
            _taskService = taskService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTask createTask)
        {
            try
            {
                var createdTask = await _taskService.CreateTaskAsync(createTask);

                return Ok(new
                {
                    Success = true,
                    Message = "Task created successfully.",
                    Data = createdTask
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while creating the task."
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(Guid id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);

            if (task == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = $"Task with ID {id} not found."
                });
            }

            return Ok(new
            {
                Success = true,
                Data = task
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await _taskService.GetAllTasksAsync();

            return Ok(new
            {
                Success = true,
                Data = tasks
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTask taskUpdate)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = $"Task with ID {id} not found."
                });
            }

            task.Title = taskUpdate.Title;
            task.Description = taskUpdate.Description;
            task.AssignedUserId = taskUpdate.AssignedUserId;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = "Task updated successfully.",
                Data = task
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = $"Task with ID {id} not found."
                });
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = "Task deleted successfully."
            });
        }
    }
}