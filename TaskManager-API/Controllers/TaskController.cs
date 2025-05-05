using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager_API.Data;
using TaskManager_API.Models.Domain;

namespace TaskManager_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly TaskContext _context;
        private readonly UserContext _userContext;
        private readonly ILogger<TasksController> _logger;

        public TasksController(TaskContext context, UserContext userContext, ILogger<TasksController> logger)
        {
            _context = context;
            _userContext = userContext;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskItem task)
        {
            if (task == null || string.IsNullOrWhiteSpace(task.Title))
                return BadRequest("Invalid task data.");

            var assignedUser = await _userContext.Users.FirstOrDefaultAsync(u => u.Id == task.AssignedUserId);
            if (assignedUser == null)
                return NotFound($"No user found with ID {task.AssignedUserId}");

            task.Id = Guid.NewGuid();
            task.CreatedDate = DateTime.UtcNow;
            task.User = assignedUser;

            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = "Task created successfully.",
                Data = task
            });
        }
    }
}