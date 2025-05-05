using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager_API.Data;

namespace TaskManager_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly TaskContext _taskContext;
        private readonly UserContext _userContext;
        private readonly ILogger<TestController> _logger;
        private readonly IConfiguration _configuration;

        public TestController(
            TaskContext taskContext,
            UserContext userContext,
            ILogger<TestController> logger,
            IConfiguration configuration)
        {
            _taskContext = taskContext;
            _userContext = userContext;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                var tasksCount = await _taskContext.Tasks.CountAsync();
                var usersCount = await _userContext.Users.CountAsync();

                return Ok(new
                {
                    Success = true,
                    Message = "Sucessfull connection.",
                    TasksCount = tasksCount,
                    UsersCount = usersCount,
                    DatabaseSettings = new
                    {
                        Endpoint = _configuration["CosmosDb:Endpoint"],
                        Database = _configuration["CosmosDb:Database"],
                        TasksContainer = _configuration["CosmosDb:Containers:Tasks"],
                        UsersContainer = _configuration["CosmosDb:Containers:Users"]
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Connection error to Cosmos DB: {ex.Message}",
                    Details = ex.ToString(),
                    DatabaseSettings = new
                    {
                        Endpoint = _configuration["CosmosDb:Endpoint"] ?? "Without config",
                        Database = _configuration["CosmosDb:Database"] ?? "Without config",
                        TasksContainer = _configuration["CosmosDb:Containers:Tasks"] ?? "Without config",
                        UsersContainer = _configuration["CosmosDb:Containers:Users"] ?? "Without config"
                    }
                });
            }
        }
    }
}