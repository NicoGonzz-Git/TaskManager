using Microsoft.AspNetCore.Mvc;
using TaskManager_API.Data;
using TaskManager_API.Services;
using TaskManager_API.DTOs;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TaskManager_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize()]
    public class TasksController : ControllerBase
    {
        private readonly TaskContext _context;
        private readonly TaskUserService _taskService;
        private readonly ILogger<TasksController> _logger;
        private readonly IMapper _mapper;
        public TasksController(
            TaskContext context,
            TaskUserService taskService,
            ILogger<TasksController> logger,
            IMapper mapper)
        {
            _context = context;
            _taskService = taskService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTask createTask)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var createdTask = await _taskService.CreateTaskAsync(createTask);
                    var taskDto = _mapper.Map<TasksDto>(createdTask);

                    return Ok(new
                    {
                        Success = true,
                        Message = "Task created successfully.",
                        Data = taskDto
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
                        Message = ex.Message
                    });
                }
            }
            else
            {
                return BadRequest(ModelState);
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
        [Authorize(Roles = "admin,User")]
        public async Task<IActionResult> GetAllTasks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.IsInRole("admin") ? "admin" : "User";
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            try
            {
                if (userRole == "admin")
                {
                    var allTasks = await _taskService.GetAllTasksAsync();
                    return Ok(new { Success = true, Data = allTasks });
                }
                else
                {
                    var userTasks = await _taskService.GetTasksByUserEmailAsync(userEmail);
                    var taskDtos = _mapper.Map<IEnumerable<TasksDto>>(userTasks);
                    return Ok(new { Success = true, Data = taskDtos });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("pagination")]
        public async Task<IActionResult> GetAllTasksPagination(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var totalTasks = await _context.Tasks.CountAsync();

            var totalPages = (int)Math.Ceiling(totalTasks / (double)pageSize);

            var tasks = await _context.Tasks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var tasksDto = _mapper.Map<IEnumerable<TasksDto>>(tasks);

            return Ok(new
            {
                Success = true,
                Data = tasksDto,
                Pagination = new
                {
                    TotalItems = totalTasks,
                    TotalPages = totalPages,
                    CurrentPage = page,
                    PageSize = pageSize
                }
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

            _mapper.Map(taskUpdate, task);

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